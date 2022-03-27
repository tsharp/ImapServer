using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DarkSpace.MailService.Abstractions;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace DarkSpace.ImapServer.Core
{
    public class ImapSessionContext : IDisposable
    {
        private readonly IMailService mailService;
        X509Certificate2 certificate = new X509Certificate2();
        public const int DefaultBufferSize = 1024;
        private int sendCount = 0;

        private readonly TcpClient client;
        private Stream clientStrem;
        private ClaimsPrincipal principal = new ClaimsPrincipal();

        public readonly Guid ContextId = Guid.NewGuid();

        public bool SslEnabled { get; internal set; } = false;

        public event EventHandler<string> Sent;
        public event EventHandler<string> Recieved;

        public IMailService MailService => mailService;
        public ClaimsPrincipal Principal => principal;

        public bool AuthenticateUser(string userName, string password)
        {
            try
            {
                this.principal = mailService.AuthenticateUser(userName, password);
            }
            catch
            {
            }

            return principal.Identity.IsAuthenticated;
        }

        private static X509Certificate2 ConvertToWindows(Org.BouncyCastle.X509.X509Certificate newCert, AsymmetricCipherKeyPair kp)
        {
            var tempStorePwd = "FlyingGoats4545"; // RandomGenerators.GetRandomString(50, 75);
            var tempStoreFile = new FileInfo(Path.GetTempFileName());

            try
            {
                // store key 
                {
                    var newStore = new Pkcs12Store();

                    var certEntry = new X509CertificateEntry(newCert);

                    newStore.SetCertificateEntry(
                        Environment.MachineName,
                        certEntry
                        );

                    newStore.SetKeyEntry(
                        Environment.MachineName,
                        new AsymmetricKeyEntry(kp.Private),
                        new[] { certEntry }
                        );

                    using (var s = tempStoreFile.Create())
                    {
                        newStore.Save(
                            s,
                            tempStorePwd.ToCharArray(),
                            new SecureRandom(new CryptoApiRandomGenerator())
                            );
                    }
                }

                // reload key 
                return new X509Certificate2(tempStoreFile.FullName, tempStorePwd);
            }
            finally
            {
                tempStoreFile.Delete();
            }
        }

        static X509Certificate2 GenerateCertificate(string certName)
        {
            var keypairgen = new RsaKeyPairGenerator();
            keypairgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));

            var keypair = keypairgen.GenerateKeyPair();

            var gen = new X509V3CertificateGenerator();

            var CN = new X509Name("CN=" + certName);
            var SN = BigInteger.ProbablePrime(120, new Random());

            gen.SetSerialNumber(SN);
            gen.SetSubjectDN(CN);
            gen.SetIssuerDN(CN);
            gen.SetNotAfter(DateTime.MaxValue);
            gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            gen.SetSignatureAlgorithm("SHA256withRSA");
            gen.SetPublicKey(keypair.Public);

            var newCert = gen.Generate(keypair.Private);

            return ConvertToWindows(newCert, keypair);

            // var cert = new X509Certificate2(DotNetUtilities.ToX509Certificate(newCert));
        }

        public ImapSessionContext(TcpClient client, IMailService mailService)
        {
            this.mailService = mailService;
            this.client = client;
            clientStrem = this.client.GetStream();
        }

        public void WriteLine(string data)
        {
            var content = data.Replace("{sendCount}", sendCount.ToString());

            using (var writer = new StreamWriter(clientStrem, Encoding.ASCII, DefaultBufferSize, true) { AutoFlush = true, NewLine = "\r\n" })
            {
                writer.WriteLine(content);
                writer.Flush();
            }

            OnSent(content);
        }

        public string ReadLine()
        {
            using (var reader = new StreamReader(clientStrem, Encoding.UTF8, true, DefaultBufferSize, true) { })
            {
                var task = reader.ReadLineAsync();
                task.Wait(5000);
                string recieved = task.Result;
                OnRecieved(recieved);
                return recieved;
            }
        }

        public void Dispose()
        {
            // TODO: Test reader/writer memory leaks
            clientStrem.Dispose();
            client.Close();
        }

        public async Task UpgradeToSsl()
        {
            SslStream sslStream = new SslStream(clientStrem, true);
            await sslStream.AuthenticateAsServerAsync(GenerateCertificate("NewCert"), false, System.Security.Authentication.SslProtocols.Tls12, false);
            clientStrem = sslStream;
            SslEnabled = true;
        }

        protected virtual void OnSent(string command)
        {
            sendCount++;
            Sent?.Invoke(this, command);
        }

        protected virtual void OnRecieved(string command)
        {
            Recieved?.Invoke(this, command);
        }
    }
}

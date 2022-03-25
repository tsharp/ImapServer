using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace ImapServer
{
    public class ImapSessionContext : IDisposable
    {
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

        X509Certificate2 certificate = new X509Certificate2();
        public const int DefaultBufferSize = 1024;

        private readonly TcpClient _client;
        private Stream _clientStream;
        
        public readonly Guid ContextId = Guid.NewGuid();

        public bool SslEnabled { get; internal set; } = false;

        public event EventHandler<string> Sent;
        public event EventHandler<string> Recieved;

        public ImapSessionContext(TcpClient client)
        {
            _client = client;
            _clientStream = _client.GetStream();
        }

        public void WriteLine(string data)
        {
            var writer = new StreamWriter(_clientStream, Encoding.ASCII, DefaultBufferSize, true) { AutoFlush = true, NewLine = "\r\n" };
            writer.WriteLine(data);
            writer.Flush();
            OnSent(data);
        }

        public string ReadLine()
        {
            var reader = new StreamReader(_clientStream, Encoding.UTF8, true, DefaultBufferSize, true) { };
            var task = reader.ReadLineAsync();
            task.Wait(5000);
            string recieved = task.Result;
            OnRecieved(recieved);
            return recieved;
        }

        public void Dispose()
        {
            // TODO: Test reader/writer memory leaks
            _clientStream.Dispose();
            _client.Close();
        }

        public async Task UpgradeToSsl()
        {
            SslStream sslStream = new SslStream(_clientStream, true);
            await sslStream.AuthenticateAsServerAsync(GenerateCertificate("NewCert"), false, System.Security.Authentication.SslProtocols.Tls12, false);
            _clientStream = sslStream;
            SslEnabled = true;
        }

        protected virtual void OnSent(string command)
        {
            Sent?.Invoke(this, command);
        }

        protected virtual void OnRecieved(string command)
        {
            Recieved?.Invoke(this, command);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ImapServer.Protocol
{
    public class AuthenticateCommand : ImapCommand
    {
        public AuthenticateCommand() : base("AUTHENTICATE", 1)
        {
        }

        public override bool CanParse(string command)
        {
            return base.CanParse(command);
        }

        public static byte[][] Separate(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    Part = new byte[I - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts.ToArray();
        }

        private static bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            ImapCommand.Send(context, "+", string.Empty);
            var credentials = context.ReadLine();

            if (credentials == "*" || string.IsNullOrWhiteSpace(credentials))
            {
                ImapCommand.Send(context, "BAD", string.Empty);
            }

            var decodedBytes = Convert.FromBase64String(credentials);
            var result = Encoding.UTF8.GetString(decodedBytes).Split('\0');
            var userName = result[1];
            var password = result[2];

            GenericCommand.Execute(context, "OK", "Success", args);
        }
    }
}

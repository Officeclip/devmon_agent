using System;
using System.Text;

namespace Geheb.DevMon.Agent.Core
{
    internal sealed class Base64Url
    {
        public string Decode(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            string incoming = input.Replace('_', '/').Replace('-', '+');
            switch (input.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(incoming);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}

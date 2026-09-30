using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DOL
{

    public sealed class LaunchArguments
    {
        public FileInfo GameDll { get; set; }

        public string Host { get; set; }

        public ushort Port { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string Character { get; set; }

        public string Realm { get; set; }

        public EncryptionType _EncryptionType { get; set; }

        public string ResolvedHost
        {
            get
            {
                if (IPAddress.TryParse(Host, out _))
                    return Host;

                return Dns.GetHostAddresses(Host)
                    .First(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .ToString();
            }
        }

        public LaunchArguments()
        {
            _EncryptionType = EncryptionType.None;
        }
    }
}

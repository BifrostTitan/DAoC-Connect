using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace DOL
{
    public static class Program
    {
        private const ushort DefaultPort = 10300;
        private const string GameDllName = "game.dll";


        public static int Main(string[] args)
        {
            LaunchArguments arguments =
                new LaunchArguments();

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];

                    switch (arg)
                    {
                        case "-e":
                        case "--rc4":

                            if (arguments._EncryptionType != EncryptionType.None)
                            {
                                throw new ArgumentException(
                                    "-e|--rc4 can't be enabled with other encryption scheme!");
                            }

                            arguments._EncryptionType =
                                EncryptionType.Rc4;

                            break;

                        case "-a":
                        case "--rsarc4":

                            if (arguments._EncryptionType != EncryptionType.None)
                            {
                                throw new ArgumentException(
                                    "-a|--rsarc4 can't be enabled with other encryption scheme!");
                            }

                            arguments._EncryptionType =
                                EncryptionType.RsaRc4;

                            break;

                        default:

                            if (arguments.GameDll == null)
                            {
                                FileInfo gameDll =
                                    FindGameDLL(arg);

                                if (!gameDll.Exists)
                                {
                                    throw new ArgumentException(
                                        $"Wrong argument value : game.dll does not exists ({gameDll.FullName})");
                                }

                                arguments.GameDll = gameDll;
                                break;
                            }

                            if (arguments.Host == null)
                            {
                                Uri uri =
                                    new Uri($"tcp://{arg}");

                                arguments.Host =
                                    uri.Host;

                                arguments.Port =
                                    uri.Port > 0
                                        ? (ushort)uri.Port
                                        : DefaultPort;

                                break;
                            }

                            if (arguments.Account == null)
                            {
                                arguments.Account = arg;
                                break;
                            }

                            if (arguments.Password == null)
                            {
                                arguments.Password = arg;
                                break;
                            }

                            if (arguments.Character == null)
                            {
                                arguments.Character = arg;
                                break;
                            }

                            if (arguments.Realm == null)
                            {
                                arguments.Realm = arg;
                                break;
                            }

                            break;
                    }
                }

                if (arguments.GameDll == null)
                    throw new ArgumentException(
                        "Missing argument : game.dll");

                if (arguments.Host == null)
                    throw new ArgumentException(
                        "Missing argument : server host/port");

                if (arguments.Account == null)
                    throw new ArgumentException(
                        "Missing argument : account");

                if (arguments.Password == null)
                    throw new ArgumentException(
                        "Missing argument : password");
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);

                ShowUsage();

                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Exception while parsing arguments :{1} {0}",
                    ex,
                    Environment.NewLine);

                ShowUsage();

                return 2;
            }

            try
            {
                Console.WriteLine("Launching patch...");
                new Launcher(arguments).Start();
                Console.ReadKey();
                return 0;
            }
            catch (SocketException ex)
            {
                Console.Error.WriteLine(
                    "Network Error: {0}",
                    ex.Message);

                return 3;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Exception while running Connect Sequence :{1} {0}",
                    ex,
                    Environment.NewLine);

                return 1;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("connect.exe usage : ");

            Console.WriteLine(
                " - connect.exe \"<path_to_game.dll>\" <server[:port]> <account> <password> [<character> <realm>]");

            Console.WriteLine(
                " - connect.exe [<-e|--rc4> | <-a|--rsarc4>] \"<path_to_game.dll>\" <server[:port]> <account> <password> [<character> <realm>]");
        }

        public static FileInfo FindGameDLL(string hint)
        {
            return new[]
            {
                hint,
                Path.Combine(hint, "game.dll"),
                Path.Combine(
                    Path.Combine(hint, ".."),
                    "game.dll")
            }
            .Select(x => new FileInfo(x))
            .FirstOrDefault(x => x.Exists)
            ?? new FileInfo(hint);
        }
    }
}
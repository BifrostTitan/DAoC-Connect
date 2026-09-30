using System;
using System.Collections.Generic;
using System.Linq;

namespace DOL
{
    internal sealed class Launcher
    {
        private readonly LaunchArguments _arguments;
        private readonly int _clientVersion;
        private readonly int _buildRevision;

        public Launcher(LaunchArguments arguments)
        {
            _arguments = arguments;

            _clientVersion = 156;

            _buildRevision =
                ClientHelper.Revision;
        }

        public void Start()
        {
            Console.WriteLine(
                "Starting up {0} - Version Detected : {1}",
                _arguments.GameDll.FullName,
                _clientVersion);

            IEnumerable<IPatch> patches =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t =>
                        typeof(IPatch).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract)
                    .Select(t =>
                        (IPatch)Activator.CreateInstance(t));

            using (var process = new GameProcess(_arguments))
            {
                Console.WriteLine("client version:" + _clientVersion);
               
                foreach (IPatch patch in patches)
                {
                    // Special handling for client 1.125
                    
                    if (patch.MinimumVersion == 1125 &&
                        _clientVersion == 1125)
                    {
                        if (!patch.RequiresNewRevision &&
                            _buildRevision < 64 &&
                            patch.EncryptionTypes.Contains(
                                _arguments._EncryptionType))
                        {
                            if (!patch.Apply(process))
                            {
                                Console.Error.WriteLine(
                                    "Could not Apply Patch : {0}",
                                    patch);
                            }
                        }
                        else if
                        (
                            patch.RequiresNewRevision &&
                            _buildRevision > 63 &&
                            patch.EncryptionTypes.Contains(
                                _arguments._EncryptionType)
                        )
                        {
                            if (!patch.Apply(process))
                            {
                                Console.Error.WriteLine(
                                    "Could not Apply Patch : {0}",
                                    patch);
                            }
                        }
                    }
                    else
                    {
                        if (_clientVersion >= patch.MinimumVersion &&
                            _clientVersion <= patch.MaximumVersion &&
                            patch.EncryptionTypes.Contains(
                                _arguments._EncryptionType))
                        {
                            if (!patch.Apply(process))
                            {
                                Console.Error.WriteLine(
                                    "Could not Apply Patch : {0}",
                                    patch);
                            }
                        }
                    }
                }
            }
        }
    }
}
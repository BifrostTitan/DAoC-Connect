using System.Collections.Generic;

namespace DOL
{
    public interface IPatch
    {
        int MinimumVersion { get; }
        int MaximumVersion { get; }
        bool RequiresNewRevision { get; }

        IEnumerable<EncryptionType> EncryptionTypes { get; }

        bool Apply(IProcessMemory process);
    }
}

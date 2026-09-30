using System.Collections.Generic;

namespace DOL.Patches
{
    public sealed class Rc4OnlyPre1109Patch : IPatch
    {
        private const int ScanStart = 0x410000;
        private const int ScanLength = 0x4000;

        private static readonly byte?[] Signature =
        {
            0x0F,
            0x85,
            0xF2,
            0x00,
            0x00,
            0x00,
            0x33,
            0xDB,
            0xC7,
            0x05,
            null,
            null,
            null,
            0x01,
            0x02,
            0x00,
            0x00,
            0x00,
            0x33,
            0xF6
        };

        public int MinimumVersion => int.MinValue;

        public int MaximumVersion => 1109;

        public bool RequiresNewRevision => false;

        public IEnumerable<EncryptionType> EncryptionTypes =>
            new[]
            {
                DOL.EncryptionType.Rc4
            };

        public bool Apply(IProcessMemory process)
        {
            using (var memory = new MemorySegment(process, ScanStart, ScanLength))
            {
                int address = memory.MatchingSignatureAdresses(Signature);

                if (address < 0)
                    return false;

                memory[address + 14] = 0x00;
            }

            return true;
        }

        public override string ToString()
        {
            return "RC4-Only pre-1.109";
        }
    }
}

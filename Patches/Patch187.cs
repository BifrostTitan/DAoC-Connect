using System.Collections.Generic;

namespace DOL.Patches
{
    public sealed class FreeShardPatch : IPatch
    {
        private const int ScanStart = 4497408;
        private const int ScanLength = 77824;

        private static readonly byte?[] Signature =
        {
            0x4C,
            0x05,
            0xC4,
            0x3A,
            0x4C,
            0x05,
            0xD8,
            0x75,
            0x06,
            0x40,
            0x83,
            0xF8,
            0x10,
            0x7C,
            0xF0,
            0x83,
            0xF8,
            0x10,
            0x75,
            0x07,
            0xC6,
            0x05
        };

        public int MinimumVersion => 187;

        public int MaximumVersion => 1117;

        public bool RequiresNewRevision => false;

        public IEnumerable<EncryptionType> EncryptionTypes =>
               new[]
               {
                     DOL.EncryptionType.None,
                DOL.EncryptionType.Rc4,
                DOL.EncryptionType.RsaRc4
               };

        public bool Apply(IProcessMemory process)
        {
            using (var memory = new MemorySegment(
                process,
                ScanStart,
                ScanLength))
            {
                int offset = memory.MatchingSignatureAdresses(Signature);

                if (offset < 0)
                    return false;

                // Replace:
                // 75 07  (JNZ +7)
                //
                // With:
                // 90 90  (NOP NOP)

                memory.Replace(offset + 18, 2, 0x90);
            }

            return true;
        }

        public override string ToString()
        {
            return "FreeShard 1.87+ to 1.117 Patch";
        }
    }
}
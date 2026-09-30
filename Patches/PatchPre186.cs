
using System;
using System.Collections.Generic;
using DOL;
namespace DOL.Patches
{

    public class FreeShardPre186Patch : IPatch
    {
        private const int ScanStart = 0x43D000;
        private const int ScanLength = 0x3000;

        private static readonly byte?[] Signature =
        {
            null,
            null,
            null,
            null,
            null,
            null,
            null,

            0x75,
            0x06,
            0x40,
            0x83,
            0xF8,
            0x10,
            0x7C,
            null,     // wildcard F0/EA/etc
            0x83,
            0xF8,
            0x10,
            0x75,
            0x07,
            0xC6,
            0x05
        };


        public int MinimumVersion => 168;

        public int MaximumVersion => 186;

        public bool RequiresNewRevision => false;

        public IEnumerable<EncryptionType> EncryptionTypes =>
            new[]
            {
                DOL.EncryptionType.None
            };

        public bool Apply(IProcessMemory process)
        {
            using (var memory = new MemorySegment(process, ScanStart, ScanLength))
            {
                int match = memory.MatchingSignatureAdresses(Signature);

                if (match < 0)
                {
                    Console.WriteLine("Patch failure from not finding matching signature addresses");
                    return false;
                }

                memory.Replace(match + 18, 2, 0x90);
                Console.WriteLine("Patch applied for version less than 1.86");
            }

            return true;
        }

        public override string ToString()
        {
            return "FreeShard pre-1.86";
        }
    }
}
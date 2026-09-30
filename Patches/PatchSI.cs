using System;
using System.Collections.Generic;

namespace DOL.Patches
{

public class FreeShardSIPatch : IPatch
{
        private const int ScanStart = 0x42F000;
        private const int ScanLength = 75000;

        private static readonly byte?[] Signature =
        {
            0x8A,
            0x4C,
            0x05,
            0xAC,
            0x3A,
            0x4C,
            0x05,
            0xE0,
            0x75,
            0x0B,
            0x40,
        };
        private static readonly byte?[] Signature2 =
        {
            0x8A,
            0x4C,
            0x05,
            0xA8,
            0x3A,
            0x4C,
            0x05,
            0xEC,
            0x75,
            0x08,
            0x40,
        };
        private static readonly byte?[] Signature3 =
        {
            0x8A,
            0x4C,
            0x05,
            0xB4,
            0x3A,
            0x4C,
            0x05,
            0xE4,
            0x75,
            0x0B,
            0x40,
        };
        private static readonly byte?[] Signature4 =
        {
            0x8A,
            0x4C,
            0x05,
            0xB0,
            0x3A,
            0x4C,
            0x05,
            0xE8,
            0x75,
            0x0B,
            0x40,
        };

        public int MinimumVersion => 150;

        public int MaximumVersion => 168;

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
            int match2 = memory.MatchingSignatureAdresses(Signature2);
            int match3 = memory.MatchingSignatureAdresses(Signature3);
            int match4 = memory.MatchingSignatureAdresses(Signature4);

                if (match < 0 && match2 < 0 && match3 < 0 && match4 < 0)
                {
                    Console.WriteLine("Patch failure from not finding matching signature addresses");
                    return false;
                }

                memory.Replace(match + 8, 2, 0x90);
                memory.Replace(match2 + 8, 2, 0x90);
                memory.Replace(match3 + 8, 2, 0x90);
                memory.Replace(match4 + 8, 2, 0x90);
                Console.WriteLine("Patch applied for version less than 1.68");
        }

        return true;
    }

    public override string ToString()
    {
        return "FreeShard SI";
    }
} 
}
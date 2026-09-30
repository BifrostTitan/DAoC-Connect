using System;
using System.Collections.Generic;

namespace DOL
{
    public static class SignatureReader
    {
        public static bool MatchSignature(this byte[] data, byte?[] signature, int address)
        {
            if (address >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(address), "Search address is outside of data buffer Length");
            if (signature.Length > data.Length - address)
                throw new ArgumentException("Signature length is bigger than data buffer remaining bytes...", nameof(signature));
            int num1 = address;
            foreach (byte? nullable1 in signature)
            {
                byte num2 = data[num1++];
                if (nullable1.HasValue)
                {
                    byte? nullable2 = nullable1;
                    int? nullable3 = nullable2.HasValue ? new int?((int)nullable2.GetValueOrDefault()) : new int?();
                    int num3 = (int)num2;
                    if (!(nullable3.GetValueOrDefault() == num3 & nullable3.HasValue))
                        return false;
                }
            }
            return true;
        }

        public static IEnumerable<int> MatchingSignatureAdresses(this byte[] data, byte?[] signature, int address,int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            if (address < 0 || address >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(address));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            int end = Math.Min(address + length, data.Length);

            for (int i = address; i <= end - signature.Length; i++)
            {
                if (data.MatchSignature(signature, i))
                    yield return i;
            }
        }

        public static IEnumerable<int> MatchingSignatureAdresses(this byte[] data, byte?[] signature)
        {
            return data.MatchingSignatureAdresses(signature, 0, data.Length);
        }
    } }

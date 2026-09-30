
using System.IO;
using System.Linq;

namespace DOL
{
    public static class ClientHelper
    {
        /// <summary>
        /// Client build/revision number.
        /// Used only by 1.125 clients.
        /// </summary>
        public static int Revision;

        public static int ParseVersion(FileInfo gameDll)
        {
            if (!gameDll.Exists)
                return -1;

            byte[] buffer = new byte[0x5000];

            try
            {
                using (FileStream stream = gameDll.OpenRead())
                {
                    stream.Position = 0x43000;
                    stream.Read(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                return -1;
            }

            //
            // Older client version signature
            //
            byte?[] signature1 =
            {
                0xC6,
                null,
                null,
                null,
                null,
                0x00,
                null,
                0xC6,
                null,
                null,
                null,
                null,
                0x00,
                null,
                0xC6,
                null,
                null,
                null,
                null,
                0x00,
                null,
                0xC3
            };

            int offset =
                buffer.MatchingSignatureAdresses(signature1)
                      .FirstOrDefault();

            //
            // Newer layout
            //
            if (offset == 0)
            {
                byte?[] signature2 =
                {
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x00,
                    null,
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x00,
                    null,
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x00,
                    null,
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x00,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    0xC3
                };

                offset =
                    buffer.MatchingSignatureAdresses(signature2)
                          .FirstOrDefault();
            }

            //
            // Pre-1.68 fallback?
            //
            if (offset == 0)
            {
                try
                {
                    using (FileStream stream = gameDll.OpenRead())
                    {
                        stream.Position = 1732608;
                        stream.Read(buffer, 0, buffer.Length);
                    }
                }
                catch
                {
                    return -1;
                }

                byte?[] signature3 =
                {
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x01,
                    null,
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x01,
                    null,
                    0xC6,
                    null,
                    null,
                    null,
                    null,
                    0x01,
                    null,
                    0xC3
                };

                offset =
                    buffer.MatchingSignatureAdresses(signature3)
                          .FirstOrDefault();
            }

            //
            // Build revision
            //
            Revision =
                int.Parse(
                    $"{buffer[offset + 27] - 36}");

            //
            // Client version
            //
            return offset == 0
                ? -1
                : int.Parse(
                    $"{buffer[offset + 6]}" +
                    $"{buffer[offset + 13]}" +
                    $"{buffer[offset + 20]}");
        }
    }
}
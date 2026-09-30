using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DOL
{
    internal sealed class MemorySegment : IEnumerable<byte>, IDisposable
    {
        private IProcessMemory _process;
        private int _start;
        private byte[] _buffer;

        public MemorySegment(
            IProcessMemory process,
            int start,
            int length)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (start < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(start),
                    start,
                    "Memory Segment Start should be 0 or above.");

            if (length <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    length,
                    "Memory Segment Length should be greater than 0.");

            _process = process;
            _start = start;
            _buffer = new byte[length];

            _process.ReadMemory(_start, _buffer);
        }

        public byte this[int index]
        {
            get => _buffer[index];
            set => _buffer[index] = value;
        }

        public void Replace(
            int start,
            int length,
            byte replacement)
        {
            for (int i = start; i < start + length; i++)
            {
                _buffer[i] = replacement;
            }
        }

        public int MatchingSignatureAdresses(byte?[] signature)
        {
            try
            {
                return _buffer
                    .MatchingSignatureAdresses(signature)
                    .Single();
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
        }

        public void Dispose()
        {
            _process.WriteMemory(_start, _buffer);

            _process = null;
            _buffer = null;
            _start = -1;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)_buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

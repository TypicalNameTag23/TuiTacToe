using System.Buffers;

namespace Common.Utilities;

public static class ReadOnlySequences
{
    extension(ReadOnlySequence<byte> sequence)
    {
        public short GetShort(int start)
        {
            var slice = sequence.Slice(start, sizeof(short)).FirstSpan;

            return BitConverters.ToShortEndianIndependent(slice);
        }

        public int GetInt(int start)
        {
            var slice = sequence.Slice(start, sizeof(int)).FirstSpan;

            return BitConverters.ToIntEndianIndependent(slice);
        }

        public long GetLong(int start, int length)
        {
            var slice = sequence.Slice(start, length).FirstSpan;

            return BitConverters.ToLongEndianIndependent(slice);
        }

        public ulong GetULong(int start, int length)
        {
            return (ulong) sequence.GetLong(start, length);
        }

        public string GetString(int start, int length)
        {
            return System.Text.Encoding.ASCII.GetString(sequence.Slice(start, length).FirstSpan);
        }
    }
}
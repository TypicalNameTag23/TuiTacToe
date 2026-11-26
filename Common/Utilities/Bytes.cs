namespace Common.Utilities;

public static class Bytes
{
    extension(byte[] array)
    {
        public void CopyInto(params byte[][] sources)
        {
            var offset = 0;
            foreach (var source in sources)
            {
                Buffer.BlockCopy(
                    source, 0, 
                    array, offset, source.Length);
            
                offset += source.Length;
            }
        }
    
        public void CopyInto(List<byte[]> sources)
        {
            var offset = 0;
            foreach (var source in sources)
            {
                Buffer.BlockCopy(
                    source, 0, 
                    array, offset, source.Length);
            
                offset += source.Length;
            }
        }
        
        public short GetShort(int start)
        {
            var slice = array[start..(start + sizeof(short))];

            return BitConverters.ToShortEndianIndependent(slice);
        }

        public int GetInt(int start)
        {
            var slice = array[start..(start + sizeof(int))];

            return BitConverters.ToIntEndianIndependent(slice);
        }

        public uint GetUInt(int start)
        {
            return (uint) array.GetInt(start);
        }

        public long GetLong(int start, int length)
        {
            var slice = array[start..(start + length)];

            return BitConverters.ToLongEndianIndependent(slice);
        }

        public ulong GetULong(int start, int length)
        {
            return (ulong) array.GetLong(start, length);
        }

        public string GetString(int start, int length)
        {
            return System.Text.Encoding.ASCII.GetString(array[start..(start + length)]);
        }
    }
}
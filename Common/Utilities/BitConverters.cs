namespace Common.Utilities;

public static class BitConverters
{
    public static byte[] GetBytesEndianIndependent(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        return bytes;
    }
    
    public static byte[] GetBytesEndianIndependent(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        return bytes;
    }
    
    public static byte[] GetBytesEndianIndependent(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        return bytes;
    }
    
    public static byte[] GetBytesEndianIndependent(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        return bytes;
    }
    
    public static byte[] GetBytesEndianIndependent(ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        return bytes;
    }

    public static short ToShortEndianIndependent(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 2)
        {
            throw new ArgumentException("Byte array must contain only 2 bytes when converting to a short.");
        }
        
        if (BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reversed();
        }
        
        return BitConverter.ToInt16(bytes);
    }

    public static int ToIntEndianIndependent(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
        {
            throw new ArgumentException("Byte array must contain only 4 bytes when converting to a int.");
        }
        
        if (BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reversed();
        }
        
        return BitConverter.ToInt32(bytes);
    }

    public static long ToLongEndianIndependent(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 8)
        {
            throw new ArgumentException("Byte array must contain only 8 bytes when converting to a long.");
        }
        
        if (BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reversed();
        }
        
        return BitConverter.ToInt64(bytes);
    }

    public static ulong ToULongEndianIndependent(ReadOnlySpan<byte> bytes)
    {
        return (ulong) ToLongEndianIndependent(bytes);
    }
}
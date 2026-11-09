namespace Common.Utilities;

public static class Bytes
{
    public static void CopyInto(this Array destination, params byte[][] sources)
    {
        var offset = 0;
        foreach (var source in sources)
        {
            Buffer.BlockCopy(
                source, 0, 
                destination, offset, source.Length);
            
            offset += source.Length;
        }
    }
}
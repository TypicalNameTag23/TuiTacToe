using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class ULongEncoder : AbstractValueEncoder<ulong>
{
    public override byte[] Encode(ulong data)
    {
        return BitConverters.GetBytesEndianIndependent(data);
    }
}
using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class LongEncoder : AbstractValueEncoder<long>
{
    public override byte[] Encode(long data)
    {
        return BitConverters.GetBytesEndianIndependent(data);
    }
}
using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class ShortEncoder : AbstractValueEncoder<short>
{
    public override byte[] Encode(short data)
    {
        return BitConverters.GetBytesEndianIndependent(data);
    }
}
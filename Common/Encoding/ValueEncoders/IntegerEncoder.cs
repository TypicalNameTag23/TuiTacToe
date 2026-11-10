using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class IntegerEncoder : AbstractValueEncoder<int>
{
    public override byte[] Encode(int data)
    {
        return BitConverters.GetBytesEndianIndependent(data);
    }
}
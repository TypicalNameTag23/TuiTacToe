using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class UIntEncoder : AbstractValueEncoder<uint>
{
    public override byte[] Encode(uint data)
    {
        return BitConverters.GetBytesEndianIndependent(data);
    }
}
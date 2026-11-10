using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class ShortDecoder : AbstractValueDecoder<short>
{
    public override object Decode(ReadOnlySpan<byte> data)
    {
        return BitConverters.ToShortEndianIndependent(data);
    }
}
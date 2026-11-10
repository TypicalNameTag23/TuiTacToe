using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class IntegerDecoder : AbstractValueDecoder<int>
{
    public override object Decode(ReadOnlySpan<byte> data)
    {
        return BitConverters.ToIntEndianIndependent(data);
    }
}
using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class LongDecoder : AbstractValueDecoder<long>
{
    public override object Decode(ReadOnlySpan<byte> data)
    {
        return BitConverters.ToLongEndianIndependent(data);
    }
}
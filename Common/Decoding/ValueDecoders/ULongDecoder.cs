using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class ULongDecoder : AbstractValueDecoder<ulong>
{
    public override object Decode(ReadOnlySpan<byte> data)
    {
        return BitConverters.ToULongEndianIndependent(data);
    }
}
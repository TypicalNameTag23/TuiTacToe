using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class UIntDecoder : AbstractValueDecoder<uint>
{
    public override object Decode(ReadOnlySpan<byte> data)
    {
        return (uint) BitConverters.ToIntEndianIndependent(data);
    }
}
namespace Common.Decoding.ValueDecoders;

public class ByteArrayValueDecoder : AbstractValueDecoder<byte[]>
{
    public override object Decode(ReadOnlySpan<byte> data) => data.ToArray();
}
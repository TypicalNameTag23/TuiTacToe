namespace Common.Decoding.ValueDecoders;

public class StringValueDecoder : AbstractValueDecoder<string>
{
    public override object Decode(ReadOnlySpan<byte> data)
    {
        return System.Text.Encoding.ASCII.GetString(data);
    }
}
namespace Common.Encoding.ValueEncoders;

public class ByteArrayValueEncoder : AbstractValueEncoder<byte[]>
{
    public override byte[] Encode(byte[] data) => data;
}
namespace Common.Encoding.ValueEncoders;

public class StringValueEncoder : AbstractValueEncoder<string>
{
    public override byte[] Encode(string data)
    {
        return System.Text.Encoding.ASCII.GetBytes(data);
    }
}
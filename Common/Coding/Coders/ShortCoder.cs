using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Coding.Coders;

public class ShortCoder : ValueCoder<short>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<short> value)
    {
        return BitConverters.GetBytesEndianIndependent(value.Value);
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<short> value, byte[] data)
    {
        if (data.Length < sizeof(short))
        {
            throw new CodingException($"Expected at least {sizeof(short)} bytes, but got {data.Length} instead.");
        }

        value.Value = data.GetShort(0);
        return sizeof(short);
    }
}
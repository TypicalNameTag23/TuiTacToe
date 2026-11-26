using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Coding.Coders;

public class IntegerCoder : ValueCoder<int>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<int> value)
    {
        return BitConverters.GetBytesEndianIndependent(value.Value);
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<int> value, byte[] data)
    {
        if (data.Length < sizeof(int))
        {
            throw new CodingException($"Expected at least {sizeof(int)} bytes, but got {data.Length} instead.");
        }

        value.Value = data.GetInt(0);
        return sizeof(int);
    }
}
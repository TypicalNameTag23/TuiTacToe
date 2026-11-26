using Common.Coding;
using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class LongCoder : ValueCoder<long>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<long> value)
    {
        return BitConverters.GetBytesEndianIndependent(value.Value);
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<long> value, byte[] data)
    {
        if (data.Length < sizeof(long))
        {
            throw new CodingException($"Expected at least {sizeof(long)} bytes, but got {data.Length} instead.");
        }

        value.Value = data.GetLong(0);
        return sizeof(long);
    }
}
using Common.Coding;
using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class UIntCoder : ValueCoder<uint>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<uint> value)
    {
        return BitConverters.GetBytesEndianIndependent(value.Value);
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<uint> value, byte[] data)
    {
        if (data.Length < sizeof(uint))
        {
            throw new CodingException($"Expected at least {sizeof(uint)} bytes, but got {data.Length} instead.");
        }

        value.Value = data.GetUInt(0);
        return sizeof(uint);
    }
}
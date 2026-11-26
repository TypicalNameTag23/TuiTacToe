using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Coding;

public class ValueEncoder(Dictionary<Type, IValueCoder> coders)
{
    public byte[] Encode(Codable codable)
    {
        var encodedValues = new List<byte[]>();
        var encodedDataLength = 0;
        foreach (var codableValue in codable.CodableValues)
        {
            if (!coders.TryGetValue(codableValue.Type, out var encoder))
            {
                throw new CoderNotFoundException(codableValue.Type);
            }

            var encodedValue = encoder.Encode(this, codableValue);

            encodedValues.Add(encodedValue);
            encodedDataLength += encodedValue.Length;
        }

        var encodedPacket = new byte[encodedDataLength];
        encodedPacket.CopyInto(encodedValues);

        return encodedPacket;
    }
}
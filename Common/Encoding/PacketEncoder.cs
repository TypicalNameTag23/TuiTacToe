using Common.Encoding.ValueEncoders;
using Common.Packets;
using Common.Utilities;

namespace Common.Encoding;

public class PacketEncoder(Dictionary<Type, IValueEncoder> valueEncoders)
{
    public byte[] Encode(AbstractPacket packet)
    {
        var encodedValues = new List<byte[]>();
        var encodedDataLength = 0;
        foreach (var registeredValue in packet.RegisteredValues)
        {
            if (!valueEncoders.TryGetValue(registeredValue.Type, out var encoder))
            {
                throw new Exception($"Encoder not found for type {registeredValue.Type}");
            }
            
            var encodedValue = encoder.Encode(registeredValue.Value);
            if (!registeredValue.IsFixedSize)
            {
                encodedValues.Add(BitConverter.GetBytes(encodedValue.Length));
                encodedDataLength += sizeof(int);
            }

            encodedValues.Add(encodedValue);
            encodedDataLength += encodedValue.Length;
        }

        var encodedPacket = new byte[encodedDataLength];
        encodedPacket.CopyInto(encodedValues);
        
        return encodedPacket;
    }
}
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

            byte[] encodedValue;
            if (registeredValue.IsFixedSize)
            {
                if (registeredValue.Value == null)
                {
                    throw new Exception($"Fixed size values can not be null!");
                }

                encodedValue = encoder.Encode(registeredValue.Value);
            }
            else if (registeredValue.Value == null)
            {
                encodedValue = [];
                encodedValues.Add(BitConverters.GetBytesEndianIndependent(0));
            }
            else
            {
                encodedValue = encoder.Encode(registeredValue.Value);
                if (registeredValue.Type == typeof(string))
                {
                    encodedValues.Add(BitConverters.GetBytesEndianIndependent(encodedValue.Length));
                }
                else if (registeredValue.Value is Array list)
                {
                    encodedValues.Add(BitConverters.GetBytesEndianIndependent(list.Length));
                }
                else
                {
                    throw new Exception($"Unknown type of non-fixed size {registeredValue.Type}");
                }

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
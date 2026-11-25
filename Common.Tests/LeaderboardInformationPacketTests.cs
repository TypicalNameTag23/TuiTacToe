using Common.Decoding;
using Common.Encoding;
using Common.Packets;
using Common.Packets.Leaderboard;

namespace Common.Tests;

public class Tests
{
    private PacketDecoder _decoder;
    private PacketEncoder _encoder;
    private LeaderboardInformationPacket _packet;

    [SetUp]
    public void Setup()
    {
        _encoder = PacketEncoderFactory
            .New()
            .WithDefaultEncoders()
            .Create();

        _decoder = PacketDecoderFactory
            .New()
            .WithDefaultDecoders()
            .WithPacket(PacketType.LeaderboardInformation, () => new LeaderboardInformationPacket())
            .Create();

        _packet = new LeaderboardInformationPacket(9999, new List<LeaderboardPlacement>
        {
            new("tabg is a great game", 1, 100000000),
            new("i hate tabg", 999999, 0)
        });
    }

    [Test]
    public void Test_That_Packet_Is_Encoded()
    {
        var encoded = _encoder.Encode(_packet);
        Assert.That(encoded, Is.Not.Empty);
    }
}
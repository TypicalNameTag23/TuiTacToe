using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class LeaderboardPlacementValueEncoder : AbstractValueEncoder<LeaderboardPlacement>
{
    public override byte[] Encode(LeaderboardPlacement data)
    {
        var length = BitConverters.GetBytesEndianIndependent(data.Username.Length);
        var username = System.Text.Encoding.ASCII.GetBytes(data.Username);
        var wins = BitConverters.GetBytesEndianIndependent(data.Wins);

        var encoded = new byte[length.Length + username.Length + wins.Length];
        encoded.CopyInto(length, username, wins);
        
        return encoded;
    }
}
using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class LeaderboardPlacementValueDecoder : AbstractValueDecoder<LeaderboardPlacement>
{
    // im burnt out...
    public override object Decode(ReadOnlySpan<byte> data)
    {
        var array = data.ToArray();

        var length = array.GetInt(0);
        var username = array.GetString(sizeof(int), length);
        var wins = array.GetInt(sizeof(int) + length);
        
        return new LeaderboardPlacement(username, wins);
    }
}
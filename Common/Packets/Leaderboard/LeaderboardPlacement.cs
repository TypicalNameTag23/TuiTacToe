using Common.Coding;

namespace Common;

public class LeaderboardPlacement : Codable
{
    public string Username { get; private set; }
    public int Wins { get; private set; }

    // internal ctor for encoding / decoding purposes
    internal LeaderboardPlacement() { }
    
    public LeaderboardPlacement(string username, int wins)
    {
        Username = username;
        Wins = wins;
    }

    protected override void RegisterValues()
    {
        RegisterDynamicSizeValue(nameof(Username), () => Username, v => Username = v);
        RegisterFixedSizeValue(nameof(Wins), () => Wins, v => Wins = v, sizeof(int));
    }
}
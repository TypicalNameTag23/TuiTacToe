using Server.Data.Models;

namespace Server.Data;

public class LeaderboardSource : ILeaderboardSource
{
    public async Task<List<User>> GetLeaderboardAsync(int count = 10, int offset = 0)
    {
        return [];
    }
}
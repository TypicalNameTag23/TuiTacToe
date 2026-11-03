using Server.Data.Models;

namespace Server.Data;

public interface ILeaderboardSource
{
    public Task<List<Player>> GetLeaderboardAsync(int count = 10, int offset = 0);
}
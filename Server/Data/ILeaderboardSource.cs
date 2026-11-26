using Common;
using Server.Data.Models;

namespace Server.Data;

public interface ILeaderboardSource
{
    public Task<List<LeaderboardPlacement>> GetLeaderboardAsync();
}
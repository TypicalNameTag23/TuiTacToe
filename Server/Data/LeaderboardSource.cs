using System.Data;
using Common;
using Npgsql;
using Server.Data.Models;

namespace Server.Data;

public class LeaderboardSource(NpgsqlDataSource database) : ILeaderboardSource
{
    public async Task<List<LeaderboardPlacement>> GetLeaderboardAsync()
    {
        await using var command = database.CreateCommand("""
                                                         SELECT username, wins
                                                         FROM users
                                                         ORDER BY wins DESC
                                                         LIMIT 3;
                                                         """);
        await using var reader = await command.ExecuteReaderAsync();

        var users = new List<LeaderboardPlacement>();
        while (await reader.ReadAsync())
        {
            try
            {
                users.Add(new LeaderboardPlacement(
                    reader.GetString(0),
                    reader.GetInt32(1)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return users;
    }
}
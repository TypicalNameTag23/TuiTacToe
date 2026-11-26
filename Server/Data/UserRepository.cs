using System.Data;
using Npgsql;
using NpgsqlTypes;
using Server.Data.Models;

namespace Server.Data;

public class UserRepository(NpgsqlDataSource dataSource) : IUserRepository
{
    public async Task<User?> GetUserAsync(string username)
    {
        await using var conn = dataSource.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand("""
                                                    SELECT username, password_hash, salt, wins 
                                                    FROM users 
                                                    WHERE username = $1
                                                    """, conn);
        command.Parameters.Add(new() { Value = username });
        
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        
        // idk why but pgsql is return the wrong bytes, they are correct in the db so something is happening in the process of retrieving them that messes it up
        return new User(
            reader.GetString("username"),
            (byte[]) reader[1],
            (byte[]) reader[2],
            reader.GetInt32("wins"));
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        await using var conn = dataSource.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand("""
                                                    INSERT INTO users (username, password_hash, salt, wins) 
                                                    VALUES ($1, $2, $3, $4)
                                                    """, conn);
        command.Parameters.Add(new() { Value = user.Username });
        command.Parameters.Add(new() { Value = user.PasswordHash });
        command.Parameters.Add(new() { Value = user.Salt });
        command.Parameters.Add(new() { Value = user.Wins });

        return await command.ExecuteNonQueryAsync() == 1;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        await using var conn = dataSource.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand("""
                                                    UPDATE users 
                                                    SET (username, password_hash, salt, wins) = ($1, $2, $3, $4) 
                                                    WHERE username = $5
                                                    """, conn);
        command.Parameters.Add(new() { Value = user.Username });
        command.Parameters.Add(new() { Value = user.PasswordHash });
        command.Parameters.Add(new() { Value = user.Salt });
        command.Parameters.Add(new() { Value = user.Wins });
        command.Parameters.Add(new() { Value = user.Username });

        return await command.ExecuteNonQueryAsync() == 1;
    }
}
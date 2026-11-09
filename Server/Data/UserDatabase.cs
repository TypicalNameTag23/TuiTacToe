using Server.Data.Models;

namespace Server.Data;

public class UserDatabase : IUserDatabase
{
    public Task<User> GetUserAsync(string username)
    {
        return Task.FromResult(new User(username, "asd", 10000000));
        return Task.FromResult<User?>(null);
    }

    public Task<bool> CreateUserAsync(User user)
    {
        return Task.FromResult(true);
    }

    public Task<bool> UpdateUserAsync(User user)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeleteUserAsync(User user)
    {
        return Task.FromResult(true);
    }
}
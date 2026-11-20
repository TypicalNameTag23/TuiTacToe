using Server.Data.Models;

namespace Server.Data;

public class InMemoryUserDatabase : IUserDatabase
{
    private Dictionary<string, User> _users = [];
    
    public Task<User?> GetUserAsync(string username)
    {
        return Task.FromResult(_users.GetValueOrDefault(username));
    }

    public Task<bool> CreateUserAsync(User user)
    {
        return Task.FromResult(_users.TryAdd(user.Username, user));
    }

    public Task<bool> UpdateUserAsync(User user)
    {
        if (!_users.ContainsKey(user.Username))
        {
            return Task.FromResult(false);
        }
        
        _users[user.Username] = user;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteUserAsync(User user)
    {
        return Task.FromResult(_users.Remove(user.Username));
    }
}
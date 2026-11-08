using Server.Data.Models;

namespace Server.Data;

public interface IUserDatabase
{
    public Task<User?> GetUserAsync(string username);
    public Task<bool> CreateUserAsync(User user);
    public Task<bool> UpdateUserAsync(User user);
    public Task<bool> DeleteUserAsync(User user);
}
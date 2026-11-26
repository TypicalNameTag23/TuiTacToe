using Server.Data.Models;

namespace Server.Data;

public interface IUserRepository
{
    public Task<User?> GetUserAsync(string username);
    public Task<bool> CreateUserAsync(User user);
    public Task<bool> UpdateUserAsync(User user);
}
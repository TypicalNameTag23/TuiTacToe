namespace Server.Data.Models;

public record User(string username, string passwordHash, int wins = 0);
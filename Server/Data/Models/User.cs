namespace Server.Data.Models;

public record User(string Username, string PasswordHash, string Salt, int Wins = 0);
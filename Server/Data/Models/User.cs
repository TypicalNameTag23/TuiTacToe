namespace Server.Data.Models;

public record User(string Username, string PasswordHash, int Wins = 0);
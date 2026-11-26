namespace Server.Data.Models;

public record User(string Username, byte[] PasswordHash, byte[] Salt, int Wins = 0)
{
    public string Username { get; set; } = Username;
    public byte[] PasswordHash { get; set; } = PasswordHash;
    public byte[] Salt { get; set; } = Salt;
    public int Wins { get; set; } = Wins;
}
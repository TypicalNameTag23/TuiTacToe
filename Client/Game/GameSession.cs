using Common.Packets;
using Common.Packets.Game;

namespace Client.Game;

public class GameSession(int lobbyId, ServerConnectionHandler connectionHandler, bool isHost)
{
    public int LobbyId { get; } = lobbyId;
    public bool IsPlayersTurn { get; private set; } = isHost;
    public bool IsHost { get; private set; } = isHost;
    public string OpponentUsername { get; private set; }
    public string PlayerCharacter => isHost ? "X" : "O";
    public string OpponentCharacter => isHost ? "O" : "X";
    
    public async Task<bool> TryPlayPositionAsync(short x, short y)
    {
        if (!IsPlayersTurn)
        {
            return false;
        }
        
        IsPlayersTurn = false;
        var packet = new BoardPlayedPacket(x, y);
        if (!await connectionHandler.TrySendAsync(packet))
        {
            return false;
        }

        var response = await connectionHandler.GetResponseAsync(packet);
        if (response is { PacketType: PacketType.Accepted })
        {
            return true;
        }
        
        IsPlayersTurn = true;
        return false;

    }
}
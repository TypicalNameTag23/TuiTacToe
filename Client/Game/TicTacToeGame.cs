using Common.Packets;
using Common.Packets.Game;

namespace Client.Game;

public class TicTacToeGame(PlayerType startingPlayer)
{
    private PlayerType?[][] _board =  new PlayerType?[3][];
    
    public PlayerType CurrentPlayer { get; private set; } = startingPlayer;
    public EventHandler<PlayerType>? OnPlayerWin;
    
    public bool PlayPosition(PlayerType player, short x, short y)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(2, x);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(2, y);
        ArgumentOutOfRangeException.ThrowIfLessThan(0, y);
        ArgumentOutOfRangeException.ThrowIfLessThan(0, y);
        if (CurrentPlayer != player || _board[x][y] != null)
        {
            return false;
        }
        
        _board[x][y] = player;
        CheckForWinner();
        return true;
    }

    // fuck ass ugly algorithm
    private void CheckForWinner()
    {
        // Check upper left corner
        var upperLeft = _board[0][0];
        if (upperLeft != null)
        {
            if (_board[0][1] == upperLeft && _board[0][2] == upperLeft)
            {
                // win
            }
            
            if (_board[1][0] == upperLeft && _board[2][0] == upperLeft)
            {
                // win
            }
        }
        
        var center = _board[1][1];
        if (center != null)
        {
            if (_board[1][0] == center && _board[1][2] == center)
            {
                
            }
            
            if (_board[0][1] == center && _board[2][1] == center)
            {
                
            }
            
            if (_board[0][0] == center && _board[2][2] == center)
            {
                
            }
            
            if (_board[0][2] == center && _board[2][0] == center)
            {
                
            }
        }

        var botttomRight = _board[2][2];
        if (botttomRight != null)
        {
            if (_board[2][1] == upperLeft && _board[2][0] == upperLeft)
            {
                // win
            }
            
            if (_board[1][2] == upperLeft && _board[0][2] == upperLeft)
            {
                // win
            }
        }
    }

    // public async Task<bool> TryPlayPositionAsync(short x, short y)
    // {
    //     if (!IsPlayersTurn)
    //     {
    //         return false;
    //     }
    //     
    //     IsPlayersTurn = false;
    //     var packet = new BoardPlayedPacket(x, y);
    //     if (!await connectionHandler.TrySendAsync(packet))
    //     {
    //         return false;
    //     }
    //
    //     var response = await connectionHandler.GetResponseAsync(packet);
    //     if (response is { PacketType: PacketType.Accepted })
    //     {
    //         return true;
    //     }
    //     
    //     IsPlayersTurn = true;
    //     return false;
    //
    // }
}
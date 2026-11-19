// using Common.Packets;
// using Common.Packets.Game;
//
// namespace Client.Game;
//
// public class TicTacToeGame(PlayerType startingPlayer)
// {
//     private PlayerType?[][] _board =  new PlayerType?[3][];
//     
//     public PlayerType CurrentPlayer { get; private set; } = startingPlayer;
//     public EventHandler<PlayerType>? OnGameCompleted;
//     
//     public bool PlayPosition(short x, short y)
//     {
//         if (x < 0 || x > 2 || y < 0 || y > 2 || _board[x][y] != null)
//         {
//             return false;
//         }
//
//         _board[x][y] = CurrentPlayer;
//         CheckForCompletion();
//         CurrentPlayer = CurrentPlayer == PlayerType.X 
//             ? PlayerType.O 
//             : PlayerType.X;
//
//         return true;
//     }
//
//     private void CheckForCompletion()
//     {
//         if (!_board.Any(t => t.Any(s => s == null)))
//         {
//             return;
//         }
//         
//         for (var i = 0; i < _board.Length; i++)
//         {
//             if (_board[i][0] != null && _board[i][1] == _board[i][0] && _board[i][2] == _board[i][0])
//             {
//                 OnGameCompleted?.Invoke(this, CurrentPlayer);
//             }
//             
//             if (_board[0][i] != null && _board[1][i] == _board[0][i] && _board[2][i] == _board[0][i])
//             {
//                 OnGameCompleted?.Invoke(this, CurrentPlayer);
//             }
//         }
//
//         if (_board[0][0] != null && _board[1][1] == _board[0][0] && _board[2][2] == _board[0][0])
//         {
//             OnGameCompleted?.Invoke(this, CurrentPlayer);
//         }
//
//         if (_board[0][2] != null && _board[1][1] == _board[0][2] && _board[2][0] == _board[0][2])
//         {
//             OnGameCompleted?.Invoke(this, CurrentPlayer);
//         }
//     }
// }
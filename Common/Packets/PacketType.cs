namespace Common.Packets;

/// <summary>
/// Enum describing packets types and their corresponding ids
/// </summary>
public enum PacketType : ushort
{
    #region Initial Handshake, 0-99

    /// <summary>
    /// Sent by the client to notify the server that it wants to open a connection. If the server doesn't respond, assume that it is down. 
    /// </summary>
    OpenConnection = 0,
    
    /// <summary>
    /// Sent by the client to attempt a login, server responds with Accepted if username and password exist and match, responds with Denied if not.
    /// </summary>
    Login = 1,
    
    /// <summary>
    /// Sent by the client to attempt to register a new user, server responds with Accepted if username isn’t already taken, responds with Denied if it is.
    /// </summary>
    Register = 2,
    
    #endregion

    #region Lobby Management 100-199

    /// <summary>
    /// Sent by the client to notify the server that it wants to create a lobby. The server either responds with Accepted if the user isn’t already in a lobby, or Denied if they are.
    /// </summary>
    CreateLobby = 100,
    
    /// <summary>
    /// Sent by the client to notify the server that it wants to join a lobby. The server either responds with Accepted if the user isn’t already in a lobby and the lobby exists, or Denied otherwise.
    /// </summary>
    JoinLobby = 101,
    
    /// <summary>
    /// Sent by the client to notify the server that it wants to leave a lobby. The server either responds with Accepted if the user is in a lobby, or Denied otherwise. The server should notify the other user in the lobby that this user has left. It should default to giving the user that is still in the lobby the win if a game is ongoing.
    /// </summary>
    LeaveLobby = 102,
    
    /// <summary>
    /// Sent by the server to the client to provide information about the lobby they just created. The server should create a unique lobby id that can be used to join this lobby and notify the client using this packet.
    /// </summary>
    LobbyCreationInformation = 103,
    
    /// <summary>
    /// Sent by the server to the client to provide information about the lobby they just joined. The username field should be populated by the username of the other user in the lobby.
    /// </summary>
    LobbyJoinInformation = 104,
    
    /// <summary>
    /// Sent by the server to the client to notify them that an opponent has joined their lobby. 
    /// </summary>
    OpponentConnected = 105,
    
    /// <summary>
    /// Sent by the server to the client to notify them that their opponent left the lobby. This should be sent both when the opponent explicitly leaves by sending a LeaveLobby packet and when they implicitly leave by either sending a CloseConnection packet or failing to send a Heartbeat packet on time.
    /// </summary>
    OpponentDisconnected = 106,
    
    #endregion

    #region Game Management 200-299

    /// <summary>
    /// Sent by the client to notify that it wants to place its mark on the specified spot on the board, indicated by the X and Y values. The server should respond with an Accepted packet if the client is in a game, it is the client’s turn, and the position exists and is open, otherwise it should send a Denied packet. The server should also send an UpdateBoard packet to the other client in the lobby in the case that this packet is accepted.
    /// </summary>
    BoardPlayed = 200,
    
    /// <summary>
    /// Sent by the server to notify the client in a lobby that the other player has played at a certain X and Y coord on the board.
    /// </summary>
    UpdateBoard = 201,
    
    /// <summary>
    /// Sent by the server to notify the client that the game it is currently in has completed. The Won bit indicates whether or not the client won the match.
    /// </summary>
    GameCompleted = 202,
    
    #endregion

    #region Leaderboard 300-399

    /// <summary>
    /// Send by the client to notify the server that it wants to view the leaderboard.
    /// </summary>
    ViewLeaderboard = 300,
    
    /// <summary>
    /// Sent by the server in response to a ViewLeaderboard packet. Contains the usernames and win counts of the top 5 players, ordered by ranking. Also contains the position and number of wins of the requesting player.
    /// </summary>
    LeaderboardInformation = 301,

    #endregion

    #region General 2000+

    /// <summary>
    /// Sent by the server to notify the client that its packet was received and accepted.
    /// If the HasInnerPacket field is 1, there is an inner packet that the client should decode for further information.
    /// </summary>
    Accepted = 2000,
    
    /// <summary>
    /// Sent by the server to notify the client that its packet was received and denied.
    /// </summary>
    Denied = 2001,
    
    /// <summary>
    /// Sent by either the server or the client to notify the other that it is disconnecting.
    /// If received by the client, this means that the server has forcefully disconnected it.
    /// If received by the server, this means that the client is leaving.
    /// </summary>
    CloseConnection = 2002,
    
    /// <summary>
    /// Sent by the client to the connected server to ensure that the connection is still alive and well.
    /// If no response is received, assume that the connection is stale and close it.
    /// If no heartbeat is received on the server for over 10 seconds, the server will assume that the client connection is stale and disconnect from it.
    /// </summary>
    Heartbeat = 2003,

    #endregion
}
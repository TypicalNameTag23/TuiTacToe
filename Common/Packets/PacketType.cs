namespace Common.Packets;

public enum PacketType : short
{
    #region Client Packets
    /// <summary>
    /// Packet sent by client to indicate that it wants a lobby created
    /// </summary>
    LobbyCreate = 0,
    
    /// <summary>
    /// Packet sent by client to indicate that it wants to leave a lobby
    /// </summary>
    LobbyDisconnect = 1,
    
    /// <summary>
    /// Packet sent by client to indicate that it wants to join a lobby
    /// </summary>
    LobbyJoin = 2,

    /// <summary>
    /// Packet sent by client to request the leaderboard
    /// </summary>
    ViewLeaderboard = 40,
    #endregion
    
    #region Server Packets
    /// <summary>
    /// Packet send by server to give client lobby info (id, players, etc.)
    /// </summary>
    LobbyInfo = 3,
    // LobbyJoinRejected = 4, replace with simple denied
    // LobbyJoinAccepted = 5, not needed
    

    /// <summary>
    /// Used to notify client that server received and rejected a request
    /// </summary>
    Denied = 400,
    
    /// <summary>
    /// Used for notify client that server received and processed an action that doesn't have any defined response
    /// </summary>
    Accepted = 200,
    #endregion
    
    /// <summary>
    /// Packet send by both client and server to notify the other that they are closing the connection
    /// </summary>
    CloseConnection = 20,
}
using Common.Packets.Generic;
using Common.Packets.Handshake;
using Server.Data.Models;

namespace Server;

public partial class Session
{
    private readonly Func<string, string, Task<User?>> _tryCreateUserAsync;
    private readonly Func<User, Task<bool>> _tryUpdateUserAsync;
    private readonly Func<string, string, Task<User?>> _validatePasswordAsync;
    
    public User? User { get; private set; }
    public bool Authenticated => User != null;
    
    private async Task RegisterAsync(RegisterPacket packet)
    {
        if (Authenticated)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id, "Already logged in."));
            return;
        }
        
        User = await _tryCreateUserAsync(packet.Username, packet.Password);
        if (!Authenticated)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id, "Username is taken."));
            return;
        }
          
        await _packetHandler.TrySendAsync(new AcceptedPacket(packet.Id));
    }

    private async Task LoginAsync(LoginPacket packet)
    {
        if (Authenticated)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id, "Already logged in."));
            return;
        }
        
        User = await _validatePasswordAsync(packet.Username, packet.Password);
        if (!Authenticated)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id, "Invalid username or password."));
            return;
        }
          
        await _packetHandler.TrySendAsync(new AcceptedPacket(packet.Id));
    }
}
using Mirror;

namespace MiniGolf.Network
{
    public struct UpdatePlayerListMessage : NetworkMessage
    {
        public readonly bool playerJoined;

        public UpdatePlayerListMessage(bool playerJoined)
        {
            this.playerJoined = playerJoined;
        }
    }
}
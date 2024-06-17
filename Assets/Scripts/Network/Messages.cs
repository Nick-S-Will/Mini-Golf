using Mirror;

namespace MiniGolf.Network
{
    public struct PlayerListChangedMessage : NetworkMessage
    {
        public readonly bool playerJoined;

        public PlayerListChangedMessage(bool playerJoined)
        {
            this.playerJoined = playerJoined;
        }
    }
}
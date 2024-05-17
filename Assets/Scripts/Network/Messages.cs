using Mirror;

namespace MiniGolf.Network
{
    public struct GameStartMessage : NetworkMessage
    {
        public int playerIndex;

        public GameStartMessage(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }
    }
}
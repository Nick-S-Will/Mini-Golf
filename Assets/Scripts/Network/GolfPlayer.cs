using MiniGolf.Player;
using Mirror;
using UnityEngine;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(BallController))]
    public class GolfPlayer : NetworkBehaviour
    {
        [SyncVar]
        public int index;
    }
}
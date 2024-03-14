using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }

        [Space]
        public UnityEvent OnClientEnter, OnClientExit, OnPlayerStarted, OnPlayerExitedRoom, OnPlayerIndexChanged, OnPlayerNameChanged, OnPlayerReadyChanged;

        public GolfRoomPlayer LocalPlayer => NetworkClient.localPlayer ? NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>() : null;

        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        public override void OnRoomServerPlayersReady() {}

        public override void OnRoomClientEnter()
        {
            base.OnRoomClientEnter();

            OnClientEnter.Invoke();
        }
        public override void OnRoomClientDisconnect()
        {
            base.OnRoomClientDisconnect();

            OnClientExit.Invoke();
        }
    }
}
using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Network
{
    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }

        private bool readyToStart;

        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            var startTransform = GetStartPosition();
            var position = startTransform ? startTransform.position : Vector3.zero;
            var rotation = startTransform ? startTransform.rotation : Quaternion.identity;
            var roomPlayer = Instantiate(roomPlayerPrefab.gameObject, position, rotation);

            return roomPlayer;
        }

        public override void OnRoomServerPlayersReady()
        {
            if (Utils.IsHeadless()) base.OnRoomServerPlayersReady();
            else readyToStart = true;
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            if (!Utils.IsSceneActive(RoomScene) || conn.identity == null) return;

            var roomPlayer = conn.identity.gameObject;
            SetGolfRoomPlayerVisible(roomPlayer, true);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            SetGolfRoomPlayerVisible(roomPlayer, false);

            var playerScore = gamePlayer.GetComponent<PlayerScore>();
            if (playerScore) playerScore.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
            else Debug.LogError($"{nameof(gamePlayer)} object must have {nameof(PlayerScore)} component");

            return true;
        }

        private void SetGolfRoomPlayerVisible(GameObject roomPlayer, bool visible)
        {
            var golfRoomPlayer = roomPlayer.GetComponent<GolfRoomPlayer>();
            if (golfRoomPlayer) golfRoomPlayer.SetVisible(visible);
            else Debug.LogError($"{nameof(roomPlayer)} object's {nameof(NetworkRoomPlayer)} must descend from {nameof(GolfRoomPlayer)}");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && readyToStart && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                readyToStart = false;

                ServerChangeScene(GameplayScene);
            }
        }
    }
}
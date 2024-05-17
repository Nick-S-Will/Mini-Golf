using Mirror;
using Mirror.Examples.NetworkRoom;
using UnityEngine;

namespace MiniGolf.Network
{
    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }

        private bool showStartButton;

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
            else showStartButton = true;
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            roomPlayer.SetActive(false);
            return true;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                showStartButton = false;

                ServerChangeScene(GameplayScene);
            }
        }
    }
}
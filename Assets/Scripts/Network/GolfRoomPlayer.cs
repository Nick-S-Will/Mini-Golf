using Mirror;
using UnityEngine;

namespace MiniGolf.Network
{
    public class GolfRoomPlayer : NetworkRoomPlayer
    {
        public const string PLAYER_NAME_KEY = "Player Name";

        [SyncVar(hook = nameof(NameChanged))]
        public string Name = "Loading...";

        public bool IsLeader => index == 0;

        public override void Start()
        {
            base.Start();

            GolfRoomManager.singleton.OnPlayerStarted.Invoke();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            var name = PlayerPrefs.GetString(PLAYER_NAME_KEY, "Not Found");
            CmdChangeName(name);
            
            var newTransform = NetworkManager.startPositions[0];
            transform.SetPositionAndRotation(newTransform.position, newTransform.rotation);
        }

        [Command]
        public void CmdChangeName(string name)
        {
            Name = name;
        }

        public virtual void NameChanged(string oldName, string newName) 
        { 
            GolfRoomManager.singleton.OnPlayerNameChanged.Invoke();
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            base.ReadyStateChanged(oldReadyState, newReadyState);

            if (oldReadyState == newReadyState) return;

            GolfRoomManager.singleton.ReadyStatusChanged();
            GolfRoomManager.singleton.OnPlayerReadyChanged.Invoke();
        }

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            base.IndexChanged(oldIndex, newIndex);

            var newTransform = NetworkManager.startPositions[newIndex];
            transform.SetPositionAndRotation(newTransform.position, newTransform.rotation);

            GolfRoomManager.singleton.OnPlayerIndexChanged.Invoke();
        }

        public override void OnDisable() => base.OnDisable();

        private void OnDestroy()
        {
            GolfRoomManager.singleton.OnPlayerExitedRoom.Invoke();
        }
    }
}
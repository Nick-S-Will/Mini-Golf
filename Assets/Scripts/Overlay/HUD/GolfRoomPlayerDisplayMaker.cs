using MiniGolf.Network;

namespace MiniGolf.Overlay.HUD
{
    public class GolfRoomPlayerDisplayMaker : DisplayMaker<GolfRoomPlayerDisplay, GolfRoomPlayer>
    {
        private void Start()
        {
            GolfRoomManager.singleton.OnPlayerStarted.AddListener(UpdatePlayerList);
            GolfRoomManager.singleton.OnPlayerExitedRoom.AddListener(UpdatePlayerList);
            GolfRoomManager.singleton.OnPlayerNameChanged.AddListener(UpdateDisplays);
            GolfRoomManager.singleton.OnPlayerReadyChanged.AddListener(UpdateDisplays);

            UpdatePlayerList();
        }

        public void UpdatePlayerList()
        {
            var neededDisplays = GolfRoomManager.singleton.roomSlots.Count - displayInstances.Count;
            for (int i = 0; i < neededDisplays; i++) MakeDisplay(null);

            var extraDisplays = -neededDisplays;
            for (int i = 1; i <= extraDisplays; i++) displayInstances[^i].gameObject.SetActive(false);

            for (int i = 0; i < GolfRoomManager.singleton.roomSlots.Count; i++)
            {
                var roomPlayer = (GolfRoomPlayer)GolfRoomManager.singleton.roomSlots[i];
                displayInstances[i].SetObject(roomPlayer);
                displayInstances[i].gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (GolfRoomManager.singleton == null) return;

            GolfRoomManager.singleton.OnPlayerStarted.RemoveListener(UpdatePlayerList);
            GolfRoomManager.singleton.OnPlayerExitedRoom.RemoveListener(UpdatePlayerList);
            GolfRoomManager.singleton.OnPlayerNameChanged.RemoveListener(UpdateDisplays);
            GolfRoomManager.singleton.OnPlayerReadyChanged.RemoveListener(UpdateDisplays);
        }
    }
}
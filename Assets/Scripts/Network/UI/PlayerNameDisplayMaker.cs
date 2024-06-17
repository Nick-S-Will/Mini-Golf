using MiniGolf.Player;
using Displayable;

namespace MiniGolf.Network.UI
{
    public class PlayerNameDisplayMaker : DisplayMaker<PlayerNameDisplay, PlayerScore>
    {
        protected override void Awake()
        {
            base.Awake();

            SwingController.OnStartPlayer.AddListener(AddNameDisplay);
        }

        private void Start()
        {
            GolfRoomManager.singleton.OnPlayerListChanged.AddListener(DestroyDisplaysWithNullObjects);
        }

        private void OnDestroy()
        {
            SwingController.OnStartPlayer.RemoveListener(AddNameDisplay);

            if (GolfRoomManager.singleton) GolfRoomManager.singleton.OnPlayerListChanged.RemoveListener(DestroyDisplaysWithNullObjects);
        }

        private void AddNameDisplay(SwingController player)
        {
            if (player.isLocalPlayer) return;

            var playerScore = player.GetComponent<PlayerScore>();
            MakeDisplay(playerScore);
        }
    }
}
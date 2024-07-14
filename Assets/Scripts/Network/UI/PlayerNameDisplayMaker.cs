using Displayable;
using System;

namespace MiniGolf.Network.UI
{
    public class PlayerNameDisplayMaker : DisplayMaker<PlayerNameDisplay, GolfGamePlayer>
    {
        protected override Comparison<PlayerNameDisplay> DisplayComparison => (display1, display2) => display1.DisplayObject.index - display2.DisplayObject.index;

        protected override void Awake()
        {
            base.Awake();

            GolfGamePlayer.OnStartPlayer.AddListener(AddNameDisplay);
        }

        private void Start()
        {
            if (GolfRoomManager.singleton == null)
            {
                Destroy(gameObject);
                return;
            }

            GolfRoomManager.singleton.OnPlayerListChanged.AddListener(DestroyDisplaysWithNullObjects);
        }

        private void OnDestroy()
        {
            GolfGamePlayer.OnStartPlayer.RemoveListener(AddNameDisplay);

            if (GolfRoomManager.singleton) GolfRoomManager.singleton.OnPlayerListChanged.RemoveListener(DestroyDisplaysWithNullObjects);
        }

        private void AddNameDisplay(GolfGamePlayer player)
        {
            if (player.isLocalPlayer) return;

            MakeDisplay(player);
        }
    }
}
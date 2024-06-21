using MiniGolf.Network;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGolf.UI
{
    public class SceneChangeButton : Button
    {
        public NetScene scene;

        public NetScene SelectedScene => scene;

        protected override void Awake()
        {
            base.Awake();

            onClick.AddListener(GoToScene);
        }

        private void GoToScene() => SceneManager.LoadScene(GolfRoomManager.NetSceneToName[SelectedScene]);
    }
}
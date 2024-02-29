using MiniGolf.Managers.SceneTransition;
using UnityEngine.UI;

namespace MiniGolf.Overlay.UI
{
    public class SceneChangeButton : Button
    {
        public Scene scene;

        protected override void Awake()
        {
            base.Awake();

            onClick.AddListener(GoToScene);
        }

        private void GoToScene() => SceneTransitionManager.instance.ChangeScene(scene);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGolf.Managers.SceneTransition
{
    public enum Scene { Title, Game }

    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        private static readonly string[] sceneNames = { "TitleScene", "GameScene" };

        protected override void Awake() => base.Awake();

        public static void ChangeScene(Scene scene) => ChangeScene(sceneNames[(int)scene]);
            
        public static void ChangeScene(string sceneName)
        {
            if (!instance) Debug.LogWarning($"No {nameof(SceneTransitionManager)} loaded");

            SceneManager.LoadScene(sceneName);
        }
        
        protected override void OnDestroy() => base.OnDestroy();
    }
}
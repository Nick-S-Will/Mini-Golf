using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGolf.Managers.SceneTransition
{
    public enum Scene { Title, Game }

    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        public static readonly Dictionary<Scene, string> sceneToName = new() { { Scene.Title, "TitleScene" }, { Scene.Game, "MultiplayerGameScene" } };
        
        protected override void Awake() => base.Awake();

        public static void ChangeScene(Scene scene) => ChangeScene(sceneToName[scene]);
            
        public static void ChangeScene(string sceneName)
        {
            if (!singleton) Debug.LogWarning($"No {nameof(SceneTransitionManager)} loaded");

            SceneManager.LoadScene(sceneName);
        }
        
        protected override void OnDestroy() => base.OnDestroy();
    }
}
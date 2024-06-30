using MiniGolf.Network;
using MiniGolf.Terrain.Data;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MiniGolf.Managers.Game
{
    public enum NetPlayMode { None, Singleplayer, Multiplayer }

    public class GameManager : Singleton<GameManager>
    {
        public static bool IsSingleplayer => singleton ? singleton.NetPlayMode == NetPlayMode.Singleplayer : true;
        public static bool IsMultiplayer => singleton ? singleton.NetPlayMode == NetPlayMode.Multiplayer : false;

        [SerializeField] private Course[] courseOptions;
        [Space]
        [SerializeField] [Scene] private string titleScene;

        [HideInInspector] public UnityEvent OnSelectedCourseChange;
        private int selectedCourseIndex;

        public Course[] Courses => courseOptions;
        public Course SelectedCourse => singleton.Courses[selectedCourseIndex];
        public int SelectedCourseIndex
        {
            get => selectedCourseIndex;
            set
            {
                if (value < 0 || value >= courseOptions.Length)
                {
                    Debug.LogError($"Given index ({value}) is out of range of {nameof(courseOptions)} [0, {courseOptions.Length - 1}]");
                    return;
                }

                selectedCourseIndex = value;
                OnSelectedCourseChange.Invoke();
            }
        }
        public NetPlayMode NetPlayMode { get; set; }
        public bool UsingWalls { get; set; }

        protected override void Awake()
        {
            base.Awake();

            if (courseOptions.Length == 0)
            {
                Debug.LogError($"{nameof(courseOptions)} is empty");
                return;
            }
        }

        protected override void OnDestroy() => base.OnDestroy();

        public static void Quit()
        {
            if (IsMultiplayer) GolfRoomManager.singleton.Quit();
            else SceneManager.LoadScene(singleton.titleScene);
        }

        public static void EndRound()
        {
            if (IsMultiplayer) GolfRoomManager.singleton.EndRound();
            else SceneManager.LoadScene(singleton.titleScene);
        }
    }
}
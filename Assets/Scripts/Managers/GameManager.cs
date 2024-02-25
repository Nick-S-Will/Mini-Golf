using MiniGolf.Terrain.Data;
using UnityEngine;

namespace MiniGolf.Managers.Game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Course[] courseOptions;

        private int selectedIndex;

        public Course[] Courses => courseOptions;
        public Course SelectedCourse => courseOptions[selectedIndex];
        public int SelectedIndex
        {
            get => selectedIndex;
            set => selectedIndex = (value % courseOptions.Length + courseOptions.Length) % courseOptions.Length;
        }

        protected override void Awake()
        {
            base.Awake();

            if (courseOptions.Length == 0) Debug.LogError($"{nameof(courseOptions)} is empty");

            DontDestroyOnLoad(instance.gameObject);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }
}
using MiniGolf.Terrain.Data;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Managers.Game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Course[] courseOptions;

        private Course selectedCourse;

        public Course[] Courses => courseOptions;
        public Course SelectedCourse
        {
            get => selectedCourse;
            set
            {
                if (courseOptions.Contains(value)) selectedCourse = value;
                else Debug.LogWarning($"Course '{value.Name}' not in {nameof(courseOptions)} array");
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (courseOptions.Length == 0)
            {
                Debug.LogError($"{nameof(courseOptions)} is empty");
                return;
            }
            selectedCourse = courseOptions[0];

            DontDestroyOnLoad(instance.gameObject);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }
}
using MiniGolf.Terrain.Data;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Managers.Game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Course[] courseOptions;

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

        protected override void Awake()
        {
            base.Awake();

            if (courseOptions.Length == 0)
            {
                Debug.LogError($"{nameof(courseOptions)} is empty");
                return;
            }

            DontDestroyOnLoad(singleton.gameObject);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }
}
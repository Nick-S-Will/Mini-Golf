using MiniGolf.Terrain.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Managers.Game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Course[] courseOptions;

        [HideInInspector] public UnityEvent OnSelectedCourseChange; 
        private Course selectedCourse;

        public Course[] Courses => courseOptions;
        public Course SelectedCourse
        {
            get => selectedCourse;
            set
            {
                if (!courseOptions.Contains(value))
                {
                    Debug.LogWarning($"Course '{value.Name}' not in {nameof(courseOptions)} array");
                    return;
                }

                selectedCourse = value;
                OnSelectedCourseChange.Invoke();
            }
        }
        public int SelectedIndex => Array.IndexOf(courseOptions, selectedCourse);

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
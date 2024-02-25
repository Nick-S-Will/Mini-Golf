using MiniGolf.Terrain.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Overlay.HUD
{
    public class CourseDisplay : Display<Course>
    {
        [SerializeField] private TMP_Text courseName, par, holeCount;

        protected override void Awake() => base.Awake();
        
        public override void UpdateText(Course course)
        {
            if (courseName) courseName.text = course.Name;
            if (par) par.text = course.Par.ToString();
            if (holeCount) holeCount.text = course.Length.ToString();
        }
    }
}
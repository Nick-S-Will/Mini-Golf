using MiniGolf.Terrain.Data;
using System;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class CourseDisplay : Display<Course>
    {
        [SerializeField] private TMP_Text courseName, par, holeCount;

        protected override void Awake() => base.Awake();
        
        public override void SetObject(Course course)
        {
            base.SetObject(course);

            bool isCourse = course != null;
            if (courseName) courseName.text = isCourse ? course.Name : string.Empty;
            if (par) par.text = isCourse ? course.Par.ToString(): string.Empty;
            if (holeCount) holeCount.text = isCourse ? course.Length.ToString() : string.Empty;
        }
    }
}
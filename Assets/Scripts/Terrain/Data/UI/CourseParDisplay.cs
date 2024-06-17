using Displayable;
using TMPro;
using UnityEngine;

namespace MiniGolf.Terrain.Data.UI
{
    public class CourseParDisplay : ArrayDisplay<Course, int>
    {
        [SerializeField] private TMP_Text courseNameText;
        [SerializeField] private TMP_Text courseParText;

        protected override void Awake()
        {
            base.Awake();

            if (courseNameText == null) Debug.LogError($"{nameof(courseNameText)} not assigned");
        }

        public override void SetObject(Course element)
        {
            base.SetObject(element);
            courseParText.transform.SetAsLastSibling();
        }

        public override void UpdateText()
        {
            courseNameText.text = displayObject.Name;

            base.UpdateText();

            courseParText.text = displayObject.Par.ToString();
        }
    }
}
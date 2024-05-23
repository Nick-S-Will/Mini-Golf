using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay
{
    public class ArrayDisplay<DisplayObject, ArrayType> : Display<DisplayObject> where DisplayObject : IContainer<ArrayType>
    {
        [SerializeField] private TMP_Text textPrefab;

        protected readonly List<TMP_Text> texts = new();

        protected virtual void Awake()
        {
            if (textPrefab == null) Debug.LogError($"{nameof(textPrefab)} not assigned");
        }

        public override void SetObject(DisplayObject element)
        {
            UpdateTexts(element);
            base.SetObject(element);
        }

        private void UpdateTexts(DisplayObject element)
        {
            if (element == null) return;

            int textCount = element.Length;

            var neededTextCount = textCount - texts.Count;
            for (int i = 0; i < neededTextCount; i++) texts.Add(Instantiate(textPrefab, transform));

            var extraTextCount = -neededTextCount;
            for (int i = 1; i <= extraTextCount; i++) Destroy(texts[^i].gameObject);
            if (extraTextCount > 0) texts.RemoveRange(texts.Count - extraTextCount, extraTextCount);
        }

        public override void UpdateText()
        {
            UpdateTexts(displayObject);
            for (int i = 0; i < displayObject.Length; i++)
            {
                texts[i].text = displayObject[i].ToString();
            }
        }
    }
}
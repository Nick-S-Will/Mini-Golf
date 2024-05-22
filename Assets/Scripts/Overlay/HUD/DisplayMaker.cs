using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public abstract class DisplayMaker<DisplayType, ObjectType> : MonoBehaviour where DisplayType : Display<ObjectType> where ObjectType : class
    {
        [SerializeField] protected Transform displayParent;
        [SerializeField] protected DisplayType displayPrefab;

        protected readonly List<DisplayType> displayInstances = new();

        public DisplayType[] Displays => displayInstances.ToArray();

        protected virtual void Awake()
        {
            if (displayPrefab == null) Debug.LogError($"{nameof(displayPrefab)} not assigned");
        }

        public virtual DisplayType MakeDisplay(ObjectType displayObject)
        {
            var display = Instantiate(displayPrefab, displayParent ? displayParent : transform);
            display.SetObject(displayObject);
            displayInstances.Add(display);

            return display;
        }

        public virtual void SetObjects(ObjectType[] displayObjects)
        {
            if (displayObjects == null) return;

            int displayCount = displayObjects.Length;

            var neededDisplayCount = displayCount - displayInstances.Count;
            for (int i = displayInstances.Count; i < displayCount; i++) _ = MakeDisplay(null);

            var extraDisplayCount = -neededDisplayCount;
            for (int i = 1; i <= extraDisplayCount; i++) displayInstances[^i].gameObject.SetActive(false);

            for (int i = 0; i < displayCount; i++)
            {
                displayInstances[i].SetObject(displayObjects[i]);
                displayInstances[i].gameObject.SetActive(true);
            }
        }

        public virtual void UpdateDisplays()
        {
            foreach (var display in displayInstances)
            {
                display.UpdateText();
            }
        }

        public virtual void DestroyDisplays()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            foreach (var display in Displays) contextDestroy(display.gameObject);
            displayInstances.Clear();
        }
    }
}
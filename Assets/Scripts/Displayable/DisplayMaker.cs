using System;
using System.Collections.Generic;
using UnityEngine;

namespace Displayable
{
    public abstract class DisplayMaker<DisplayType, ObjectType> : MonoBehaviour where DisplayType : Display<ObjectType> where ObjectType : class
    {
        private static Action<UnityEngine.Object> ContextDestroy => Application.isPlaying ? Destroy : DestroyImmediate;

        [SerializeField] protected Transform displayParent;
        [SerializeField] protected DisplayType displayPrefab;

        protected readonly List<DisplayType> displayInstances = new();

        protected abstract Comparison<DisplayType> DisplayComparer { get; }

        public DisplayType[] Displays => displayInstances.ToArray();

        protected virtual void Awake()
        {
            if (displayParent == null) Debug.LogError($"{nameof(displayParent)} not assigned");
            if (displayPrefab == null) Debug.LogError($"{nameof(displayPrefab)} not assigned");
        }

        public virtual void SetObjects(ObjectType[] displayObjects)
        {
            if (displayObjects == null) return;

            var extraDisplayCount = displayInstances.Count - displayObjects.Length;
            for (int i = 1; i <= extraDisplayCount; i++) displayInstances[^i].gameObject.SetActive(false);

            for (int i = 0; i < displayObjects.Length; i++)
            {
                if (i < displayInstances.Count)
                {
                    displayInstances[i].SetObject(displayObjects[i]);
                    displayInstances[i].gameObject.SetActive(true);
                }
                else _ = MakeDisplay(displayObjects[i]);
            }

            UpdateDisplays();
        }

        public virtual DisplayType MakeDisplay(ObjectType displayObject) // TODO: Revise this to reuse inactive or destroy in set objects
        {
            var display = Instantiate(displayPrefab, displayParent ? displayParent : transform);
            display.SetObject(displayObject);
            displayInstances.Add(display);

            return display;
        }

        public virtual void UpdateDisplays()
        {
            DestroyDisplaysWithNullObjects();

            displayInstances.Sort(DisplayComparer);
            int extraChildCount = displayParent.childCount - displayInstances.Count;
            for (int i = 0; i < displayInstances.Count; i++) displayInstances[i].transform.SetSiblingIndex(extraChildCount + i);

            foreach (var display in displayInstances) display.UpdateText();
        }

        protected virtual void DestroyDisplaysWithNullObjects()
        {
            foreach (var display in Displays)
            {
                if (display.DisplayObject.Equals(null)) DestroyDisplay(display);
            }
        }

        public virtual void DestroyDisplay(DisplayType display)
        {
            if (!displayInstances.Contains(display))
            {
                Debug.LogError($"Given object ({display.name}) isn't in {nameof(displayInstances)}");
                return;
            }

            displayInstances.Remove(display);
            ContextDestroy(display.gameObject);
        }

        public virtual void DestroyDisplays()
        {
            foreach (var display in Displays) ContextDestroy(display.gameObject);
            displayInstances.Clear();
        }
    }
}
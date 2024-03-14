using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public abstract class DisplayMaker<DisplayType, ObjectType> : MonoBehaviour where DisplayType : Display<ObjectType>
    {
        [SerializeField] protected Transform displayParent;
        [SerializeField] protected DisplayType displayPrefab;

        protected readonly List<DisplayType> displayInstances = new();

        public DisplayType[] Displays => displayInstances.ToArray();

        public virtual DisplayType MakeDisplay(ObjectType displayObject)
        {
            var display = Instantiate(displayPrefab, displayParent);
            display.SetObject(displayObject);
            displayInstances.Add(display);

            return display;
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
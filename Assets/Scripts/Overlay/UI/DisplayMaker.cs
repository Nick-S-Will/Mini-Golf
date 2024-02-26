using MiniGolf.Overlay.HUD;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGolf.Overlay.UI
{
    public abstract class DisplayMaker<T> : MonoBehaviour
    {
        [SerializeField] protected Transform displayParent;
        [SerializeField] protected Display<T> displayPrefab;

        protected readonly List<Display<T>> displayInstances = new();

        public Display<T>[] Displays => displayInstances.ToArray();

        public virtual Display<T> MakeDisplay(T displayObject)
        {
            var buttonDisplay = Instantiate(displayPrefab, displayParent);
            buttonDisplay.SetObject(displayObject);
            displayInstances.Add(buttonDisplay);

            return buttonDisplay;
        }

        public virtual void DestroyDisplays()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            var displays = displayInstances.ToArray();
            foreach (var display in displays) contextDestroy(display.gameObject);
            displayInstances.Clear();
        }
    }
}
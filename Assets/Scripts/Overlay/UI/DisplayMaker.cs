using MiniGolf.Overlay.HUD;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGolf.Overlay.UI
{
    public abstract class DisplayMaker<T> : MonoBehaviour
    {
        [SerializeField] protected Display<T> displayPrefab;
        [SerializeField] protected Transform displayParent;

        protected readonly List<Display<T>> displayInstances = new();

        public virtual void MakeDisplay(T element)
        {
            var buttonDisplay = Instantiate(displayPrefab, displayParent);
            displayInstances.Add(buttonDisplay);

            buttonDisplay.UpdateText(element);
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
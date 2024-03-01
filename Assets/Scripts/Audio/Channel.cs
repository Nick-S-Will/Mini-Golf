using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace MiniGolf.Audio
{
    [Serializable]
    public class Channel
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string mixerVolumeFloatName = "MasterVolume";
        [SerializeField] [Range(-80f, 20f)] private float minDecibel = -30f, maxDecibel = 10f;
        [Space]
        public UnityEvent<float> OnVolumeChange;

        private float volumePercent = 0.8f;

        public string MixerVolumeFloatName => mixerVolumeFloatName;
        public float VolumePercent => volumePercent;

        public void SetVolumePercent(float volumePercent)
        {
            this.volumePercent = volumePercent;

            var decibel = volumePercent == 0f ? -80f : PercentToDecibel(volumePercent);
            if (!audioMixer.SetFloat(mixerVolumeFloatName, decibel)) PrintMissingName();

            OnVolumeChange.Invoke(volumePercent);
        }

        public Channel(string mixerVolumeFloatName)
        {
            this.mixerVolumeFloatName = mixerVolumeFloatName;
        }

        public void ReadFromAudioMixer()
        {
            if (!audioMixer.GetFloat(mixerVolumeFloatName, out float value))
            {
                PrintMissingName();
                return;
            }

            SetVolumePercent(DecibelToPercent(value));
        }

        private float PercentToDecibel(float percent) => Mathf.Lerp(minDecibel, maxDecibel, percent);
        private float DecibelToPercent(float decibel) => Mathf.InverseLerp(minDecibel, maxDecibel, decibel);

        private void PrintMissingName() => Debug.LogError($"Given float name '{mixerVolumeFloatName}' not found in {nameof(audioMixer)}");
    }
}
using MiniGolf.Managers.Options;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace MiniGolf.Audio
{
    public enum Channel { Master, Sound, Music }

    public static class ChannelExtensions
    {
        private const string MASTER_VOLUME_KEY = "Master Volume", SOUND_VOLUME_KEY = "Sound Volume", MUSIC_VOLUME_KEY = "Music Volume";

        private static string GetPrefKey(this Channel mixerChannel)
        {
            return mixerChannel switch
            {
                Channel.Master => MASTER_VOLUME_KEY,
                Channel.Sound => SOUND_VOLUME_KEY,
                Channel.Music => MUSIC_VOLUME_KEY,
                _ => throw new NotImplementedException()
            };
        }

        public static void Initialize(this Channel channel)
        {
            var defaultValue = -1f;
            var volume = PlayerPrefs.GetFloat(channel.GetPrefKey(), defaultValue);
            if (volume == defaultValue) channel.GetMixerChannel().ReadFromAudioMixer();
            else channel.GetMixerChannel().VolumePercent = volume;

            channel.GetMixerChannel().OnVolumeChange.AddListener(volume => PlayerPrefs.SetFloat(channel.GetPrefKey(), volume));
        }

        public static MixerChannel GetMixerChannel(this Channel mixerChannel)
        {
            if (OptionsManager.singleton == null)
            {
                Debug.LogWarning($"No {nameof(OptionsManager)} loaded");
                return null;
            }

            return mixerChannel switch
            {
                Channel.Master => OptionsManager.singleton.Master,
                Channel.Sound => OptionsManager.singleton.Sound,
                Channel.Music => OptionsManager.singleton.Music,
                _ => throw new NotImplementedException()
            };
        }
    }

    [Serializable]
    public class MixerChannel
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string mixerVolumeFloatName = "MasterVolume";
        [SerializeField][Range(-80f, 20f)] private float minDecibel = -30f, maxDecibel = 10f;
        [Space]
        public readonly UnityEvent<float> OnVolumeChange;

        private float volumePercent = 0.8f;

        public string MixerVolumeFloatName => mixerVolumeFloatName;
        public float VolumePercent
        {
            get => volumePercent;
            set
            {
                volumePercent = value;
                WriteToAudioMixer();
                OnVolumeChange.Invoke(volumePercent);
            }
        }

        public MixerChannel(string mixerVolumeFloatName)
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

            VolumePercent = DecibelToPercent(value);
        }

        private void WriteToAudioMixer()
        {
            var decibel = volumePercent == 0f ? -80f : PercentToDecibel(volumePercent);
            if (!audioMixer.SetFloat(mixerVolumeFloatName, decibel)) PrintMissingName();
        }

        private float PercentToDecibel(float percent) => Mathf.Lerp(minDecibel, maxDecibel, percent);
        private float DecibelToPercent(float decibel) => Mathf.InverseLerp(minDecibel, maxDecibel, decibel);

        private void PrintMissingName() => Debug.LogError($"Given float name '{mixerVolumeFloatName}' not found in {nameof(audioMixer)}");
    }
}
using MiniGolf.Audio;
using System;
using UnityEngine;

namespace MiniGolf.Managers.Options
{
    public class OptionsManager : Singleton<OptionsManager>
    {
        [Header("Audio")]
        [SerializeField] private MixerChannel master = new("MasterVolume");
        [SerializeField] private MixerChannel sound = new("SoundVolume"), music = new("MusicVolume");

        public MixerChannel Master => master;
        public MixerChannel Sound => sound;
        public MixerChannel Music => music;

        protected override void Awake()
        {
            base.Awake();
            if (singleton != this) return;

            foreach (Channel channel in Enum.GetValues(typeof(Channel))) channel.Initialize();
        }
        
        protected override void OnDestroy() => base.OnDestroy();
    }
}
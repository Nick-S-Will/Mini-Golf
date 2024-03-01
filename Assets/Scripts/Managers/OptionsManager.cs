using MiniGolf.Audio;
using UnityEngine;

namespace MiniGolf.Managers.Options
{
    public class OptionsManager : Singleton<OptionsManager>
    {
        [Header("Audio")]
        [SerializeField] private Channel master = new("MasterVolume");
        [SerializeField] private Channel sound = new("SoundVolume"), music = new("MusicVolume");

        public Channel Master => master;
        public Channel Sound => sound;
        public Channel Music => music;

        protected override void Awake()
        {
            base.Awake();

            master.ReadFromAudioMixer();
            sound.ReadFromAudioMixer();
            music.ReadFromAudioMixer();
        }
        
        protected override void OnDestroy() => base.OnDestroy();
    }
}
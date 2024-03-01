using MiniGolf.Audio;
using MiniGolf.Managers.Game;
using MiniGolf.Managers.Options;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.UI
{
    public enum AudioChannel { Master, Sound, Music }

    public class AudioSlider : Slider
    {
        public AudioChannel audioChannel;

        protected override void Awake()
        {
            base.Awake();
            
            onValueChanged.AddListener(SetVolumePercent);

            var channel = GetChannel();
            if (channel != null) SetValueWithoutNotify(channel.VolumePercent);
            else Debug.LogWarning($"'{audioChannel}' channel not found");
        }

        private void SetVolumePercent(float volume) => GetChannel().SetVolumePercent(volume);
        
        private Channel GetChannel()
        {
            if (OptionsManager.instance == null)
            {
                Debug.LogWarning($"No {nameof(OptionsManager)} loaded");
                return null;
            }

            return audioChannel switch
            {
                AudioChannel.Master => OptionsManager.instance.Master,
                AudioChannel.Sound => OptionsManager.instance.Sound,
                AudioChannel.Music => OptionsManager.instance.Music,
                _ => throw new NotImplementedException()
            };
        }
    }
}
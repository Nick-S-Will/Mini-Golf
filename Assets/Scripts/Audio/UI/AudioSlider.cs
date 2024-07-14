using UnityEngine.UI;

namespace MiniGolf.Audio.UI
{
    public class AudioSlider : Slider
    {
        public Channel channel;

        protected override void Awake()
        {
            base.Awake();

            channel.GetMixerChannel().OnVolumeChange.AddListener(SetValueWithoutNotify);
            onValueChanged.AddListener(SetVolumePercent);
        }

        protected override void Start()
        {
            SetValueWithoutNotify(channel.GetMixerChannel().VolumePercent);
        }

        protected override void OnDestroy()
        {
            if (channel.GetMixerChannel() != null) channel.GetMixerChannel().OnVolumeChange.RemoveListener(SetValueWithoutNotify);
        }

        private void SetVolumePercent(float volume) => channel.GetMixerChannel().VolumePercent = volume;
    }
}
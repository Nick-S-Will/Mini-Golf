using UnityEngine;

namespace MiniGolf.Player.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class BackswingAudio : MonoBehaviour
    {
        [SerializeField] private SwingController swingController;
        [SerializeField][Range(0f, 1f)] private float minBackswingDeltaForSound = 0.05f;
        [SerializeField][Min(0f)] private float volumeScaler = 0.8f;

        private AudioSource audioSource;
        private float lastScaleWhenAudioPlayed;

        private void Awake()
        {
            if (swingController == null) Debug.LogError($"{nameof(swingController)} not assigned");

            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            swingController.OnBackswing.AddListener(ResetTracking);
            swingController.OnBackswingChange.AddListener(PlayAudioIfNeeded);
        }

        private void OnDisable()
        {
            if (swingController == null) return;

            swingController.OnBackswing.RemoveListener(ResetTracking);
            swingController.OnBackswingChange.RemoveListener(PlayAudioIfNeeded);
        }

        private void ResetTracking()
        {
            lastScaleWhenAudioPlayed = 0f;
        }

        private void PlayAudioIfNeeded()
        {
            var backSwingDeltaSinceSound = Mathf.Abs(swingController.BackswingScaler - lastScaleWhenAudioPlayed);
            if (backSwingDeltaSinceSound < minBackswingDeltaForSound) return;

            audioSource.volume = volumeScaler * backSwingDeltaSinceSound;
            audioSource.Play();

            lastScaleWhenAudioPlayed = swingController.BackswingScaler;
        }
    }
}
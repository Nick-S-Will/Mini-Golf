using TMPro;
using UnityEngine;

namespace MiniGolf.Network.UI
{
    public class SpectateHUD : MonoBehaviour
    {
        [SerializeField] private FreeLookSpectator spectator;
        [Space]
        [SerializeField] private GameObject graphicsParent;

        private string textPrefix;

        private void Awake()
        {
            if (spectator == null) Debug.LogError($"{nameof(spectator)} not assigned");
            if (graphicsParent == null) Debug.LogError($"{nameof(graphicsParent)} not assigned");
        }

        private void Start()
        {
            spectator.OnStartSpectating.AddListener(Show);
            spectator.OnStopSpectating.AddListener(Hide);
        }

        private void OnDestroy()
        {
            if (spectator == null) return;

            spectator.OnStartSpectating.RemoveListener(Show);
            spectator.OnStopSpectating.RemoveListener(Hide);
        }

        private void Show() => SetActive(true);
        private void Hide() => SetActive(false);
        private void SetActive(bool active) => graphicsParent.SetActive(active);
    }
}
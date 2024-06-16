using MiniGolf.Network;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class SpectateHUD : MonoBehaviour
    {
        [SerializeField] private FreeLookSpectator spectator;
        [Space]
        [SerializeField] private GameObject graphicsParent;
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private string noTargetText = "...";

        private string textPrefix;

        private void Awake()
        {
            if (spectator == null) Debug.LogError($"{nameof(spectator)} not assigned");
            if (graphicsParent == null) Debug.LogError($"{nameof(graphicsParent)} not assigned");
            if (targetText == null) Debug.LogError($"{nameof(targetText)} not assigned");

            textPrefix = targetText.text;
        }

        private void Start()
        {
            spectator.OnStartSpectating.AddListener(Show);
            spectator.OnStopSpectating.AddListener(Hide);
            spectator.OnTargetChanged.AddListener(UpdateText);
        }

        private void OnDestroy()
        {
            if (spectator == null) return;

            spectator.OnStartSpectating.RemoveListener(Show);
            spectator.OnStopSpectating.RemoveListener(Hide);
            spectator.OnTargetChanged.RemoveListener(UpdateText);
        }

        private void Show() => SetActive(true);
        private void Hide() => SetActive(false);
        private void SetActive(bool active)
        {
            graphicsParent.SetActive(active);

            if (active) UpdateText();
        }

        private void UpdateText()
        {
            var targetName = spectator.Target ? spectator.Target.Name : noTargetText;
            targetText.text = textPrefix + targetName;
        }
    }
}
using Cinemachine;
using MiniGolf.Player;
using MiniGolf.Progress;
using MiniGolf.Terrain;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Network
{
    public class FreeLookSpectator : MonoBehaviour
    {
        [SerializeField] private HoleGenerator holeGenerator;
        [SerializeField] private CinemachineFreeLook freelookCamera;
        [Space]
        [SerializeField] private PlayerInput spectateInput;
        [SerializeField][Min(0f)] private float spectateStartDelay = 1f;

        private SwingController[] playersNotInHole;
        private SwingController targetPlayer;
        private int playerIndex;
        private Coroutine spectateRoutine;

        [HideInInspector] public UnityEvent OnStartSpectating, OnTargetChanged, OnStopSpectating;

        public PlayerScore Target => targetPlayer ? targetPlayer.GetComponent<PlayerScore>() : null;
        public int TargetCount => playersNotInHole != null ? playersNotInHole.Length : 0;
        public bool IsSpectating => spectateRoutine != null;

        private void Awake()
        {
            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");
            if (freelookCamera == null) Debug.LogError($"{nameof(freelookCamera)} not assigned");
            if (spectateInput == null) Debug.LogError($"{nameof(spectateInput)} not assigned");
        }

        private void Start()
        {
            ProgressHandler.singleton.OnPlayerWaiting.AddListener(Spectate);
            ProgressHandler.singleton.OnCompleteHole.AddListener(StopSpectating);
        }

        private void Spectate() => spectateRoutine ??= StartCoroutine(SpectateRoutine());
        private IEnumerator SpectateRoutine()
        {
            var players = FindObjectsOfType<SwingController>();
            var holeTile = holeGenerator.CurrentHoleTile;

            OnStartSpectating.Invoke();

            spectateInput.enabled = true;

            SwingController[] GetPlayersNotInHole() => players.Where(player => player && !holeTile.Contains(player)).ToArray();
            playersNotInHole = GetPlayersNotInHole();
            targetPlayer = null;
            while (this && TargetCount > 0)
            {
                WrapIndex();
                var newTargetPlayer = playersNotInHole[playerIndex];
                if (targetPlayer != newTargetPlayer)
                {
                    targetPlayer = newTargetPlayer;

                    if (targetPlayer == null) yield return new WaitForSeconds(spectateStartDelay);

                    if (targetPlayer && !holeTile.Contains(targetPlayer))
                    {
                        freelookCamera.Follow = targetPlayer.transform;
                        freelookCamera.LookAt = targetPlayer.transform;

                        OnTargetChanged.Invoke();
                    }
                }

                yield return null;

                playersNotInHole = GetPlayersNotInHole();
            }

            StopSpectating();
        }

        private void WrapIndex() => playerIndex = ((playerIndex % TargetCount) + TargetCount) % TargetCount;

        private void StopSpectating()
        {
            spectateInput.enabled = false;

            if (PlayerHandler.Player == null) return;

            freelookCamera.Follow = PlayerHandler.Player.transform;
            freelookCamera.LookAt = PlayerHandler.Player.transform;

            playersNotInHole = null;
            targetPlayer = null;
            playerIndex = 0;

            if (IsSpectating)
            {
                StopCoroutine(spectateRoutine);
                spectateRoutine = null;

                OnStopSpectating.Invoke();
            }
        }

        public void SpectateNextTarget(InputAction.CallbackContext context)
        {
            if (context.started) IncrementTargetIndex(1);
        }
        public void SpectatePreviousTarget(InputAction.CallbackContext context)
        {
            if (context.started) IncrementTargetIndex(-1);
        }

        private void IncrementTargetIndex(int amount)
        {
            playerIndex += amount;
        }
    }
}
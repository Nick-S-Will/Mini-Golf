using Cinemachine;
using MiniGolf.Player;
using MiniGolf.Progress;
using MiniGolf.Terrain;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    public class FreeLookSpectator : MonoBehaviour
    {
        [SerializeField] private HoleGenerator holeGenerator;
        [SerializeField] private CinemachineFreeLook freelookCamera;
        [Space]
        [SerializeField][Min(0f)] private float spectateSwapDelay = 1f;
        
        private SwingController targetPlayer;
        private Coroutine spectateRoutine;

        [HideInInspector] public UnityEvent OnStartSpectating, OnTargetChanged, OnStopSpectating;

        public PlayerScore Target => targetPlayer ? targetPlayer.GetComponent<PlayerScore>() : null;
        public bool IsSpectating => spectateRoutine != null;

        private void Awake()
        {
            if (freelookCamera == null) Debug.LogError($"{nameof(freelookCamera)} not assigned");
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
            targetPlayer = null;

            OnStartSpectating.Invoke();

            SwingController[] GetPlayersNotInHole() => players.Where(player => player && !holeTile.Contains(player)).ToArray();
            var playersNotInHole = GetPlayersNotInHole();
            while (this && playersNotInHole.Length > 0)
            {
                if (targetPlayer != playersNotInHole.First())
                {
                    targetPlayer = playersNotInHole.First();

                    yield return new WaitForSeconds(spectateSwapDelay);

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

        private void StopSpectating()
        {
            freelookCamera.Follow = PlayerHandler.Player.transform;
            freelookCamera.LookAt = PlayerHandler.Player.transform;
            targetPlayer = null;

            if (IsSpectating)
            {
                StopCoroutine(spectateRoutine);
                spectateRoutine = null;

                OnStopSpectating.Invoke();
            }
        }
    }
}
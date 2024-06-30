using MiniGolf.Player;
using MiniGolf.Managers.Game;
using MiniGolf.Terrain;
using MiniGolf.Terrain.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MiniGolf.Network;

namespace MiniGolf.Progress
{
    public class ProgressHandler : MonoBehaviour // TODO: Remove network dependency
    {
        [Space]
        [SerializeField] private HoleGenerator holeGenerator;
        [Space]
        [SerializeField] private Vector3 spawnPosition = Vector3.up;
        [SerializeField] private float ballMinY = -10f;
        [Space]
        [SerializeField][Min(0f)] private float holeEndTime = 2f;
        [SerializeField][Min(0f)] private float courseEndTime = 5f;
        [Space]
        public UnityEvent OnStartCourse, OnStartHole;
        public UnityEvent OnStrokeAdded, OnFallOff, OnPlayerWaiting;
        public UnityEvent OnCompleteHole, OnCompleteCourse;

        private HoleTile holeTile;
        private Course course;
        private Coroutine completeHoleRoutine;
        /// <summary>Array of position lists for each hole</summary>
        private List<Vector3>[] holePositions = new List<Vector3>[0];
        private int holeIndex;

        public HoleGenerator HoleGenerator => holeGenerator;
        public int[] Scores => holePositions.Select(positions => positions.Count()).ToArray();
        public int CurrentScore => holeIndex < holePositions.Length ? CurrentPositions.Count : 0;
        private List<Vector3> CurrentPositions => holePositions[holeIndex];

        private void Awake()
        {
            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");

            PlayerHandler.OnSetPlayer.AddListener(ChangePlayer);
            PlayerHandler.OnPlayerReady.AddListener(BeginCourse);
            holeGenerator.OnGenerate.AddListener(UpdateHoleTile);
        }

        private void Start()
        {
            if (GameManager.IsMultiplayer) GolfRoomManager.singleton.OnPlayerListChanged.AddListener(TryCompleteHole);
        }

        private void OnDestroy()
        {
            if (PlayerHandler.singleton) PlayerHandler.OnSetPlayer.RemoveListener(ChangePlayer);
            if (PlayerHandler.Player) PlayerHandler.Player.OnSwing.RemoveListener(AddStroke);
            if (holeGenerator) holeGenerator.OnGenerate.RemoveListener(UpdateHoleTile);
            if (holeTile) holeTile.OnBallEnter.RemoveListener(HoleBall);
            if (GolfRoomManager.singleton) GolfRoomManager.singleton.OnPlayerListChanged.RemoveListener(TryCompleteHole);
        }

        private void FixedUpdate()
        {
            CheckFall();
        }

        private void ChangePlayer(SwingController oldPlayer, SwingController newPlayer)
        {
            if (oldPlayer) oldPlayer.OnSwing.RemoveListener(AddStroke);
            if (newPlayer) newPlayer.OnSwing.AddListener(AddStroke);
        }

        private void UpdateHoleTile(HoleTile newHoleTile)
        {
            if (holeTile) holeTile.OnBallEnter.RemoveListener(HoleBall);

            holeTile = newHoleTile;
            if (holeTile) holeTile.OnBallEnter.AddListener(HoleBall);
        }

        private void BeginCourse()
        {
            if (GameManager.singleton == null) Debug.LogWarning($"No {nameof(GameManager)} loaded");
            course = GameManager.singleton ? GameManager.singleton.SelectedCourse : new Course();
            holePositions = new List<Vector3>[course.Length];
            for (int i = 0; i < holePositions.Length; i++) holePositions[i] = new();

            if (TryBeginHole()) OnStartCourse.Invoke();
            else Debug.LogWarning("Course couldn't be started");

            PlayerHandler.OnPlayerReady.RemoveListener(BeginCourse);
        }

        private bool TryBeginHole()
        {
            if (holeIndex >= course.Length) return false;

            holeGenerator.Generate(course.HoleData[holeIndex]);

            var position = GameManager.IsMultiplayer ? GolfRoomManager.singleton.GetHoleStartPosition() : spawnPosition;
            PlayerHandler.Player.transform.SetPositionAndRotation(position, Quaternion.identity);
            if (holeIndex > 0) PlayerHandler.SwingControlsEnabled = true;
            
            OnStartHole.Invoke();
            return true;
        }

        private void AddStroke()
        {
            holePositions[holeIndex].Add(PlayerHandler.Player.transform.position);
            OnStrokeAdded.Invoke();
        }

        private void CheckFall()
        {
            if (PlayerHandler.Player == null || PlayerHandler.Player.Rigidbody.transform.position.y > ballMinY) return;

            PlayerHandler.Player.transform.position = CurrentPositions[^1];
            PlayerHandler.Player.Rigidbody.velocity = Vector3.zero;
            PlayerHandler.Player.Rigidbody.angularVelocity = Vector3.zero;

            OnFallOff.Invoke();
        }

        private void HoleBall(SwingController ball)
        {
            var isPlayer = PlayerHandler.Player == ball;
            if (isPlayer) PlayerHandler.SwingControlsEnabled = false;
            
            if (CanChangeHoles()) CompleteHole();
            else if (isPlayer) OnPlayerWaiting.Invoke();
        }

        private void TryCompleteHole()
        {
            if (CanChangeHoles()) CompleteHole();
        }

        private bool CanChangeHoles()
        {
            if (holeTile == null) return false;
            
            if (GameManager.IsSingleplayer) return holeTile.BallCount == 1; 
            else return holeTile.BallCount == GolfRoomManager.singleton.roomSlots.Count;
        }

        private void CompleteHole() => completeHoleRoutine ??= StartCoroutine(CompleteHoleRoutine());
        private IEnumerator CompleteHoleRoutine()
        {
            OnCompleteHole.Invoke();

            yield return new WaitForSeconds(holeEndTime);

            holeIndex++;
            if (!TryBeginHole()) yield return StartCoroutine(CompleteCourseRoutine());

            completeHoleRoutine = null;
        }

        private IEnumerator CompleteCourseRoutine()
        {
            OnCompleteCourse.Invoke();
            yield return new WaitForSeconds(Mathf.Max(courseEndTime - holeEndTime, 0f));

            GameManager.EndRound();
        }
    }
}
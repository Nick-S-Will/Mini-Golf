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
using Mirror;

namespace MiniGolf.Progress
{
    public class ProgressHandler : Singleton<ProgressHandler> // TODO: Check if singleton is necessary
    {
        [Space]
        [SerializeField] private HoleGenerator holeGenerator;
        [Space]
        [SerializeField] private float ballMinY = -10f;
        [Space]
        [SerializeField][Min(0f)] private float holeEndTime = 2f;
        [SerializeField][Min(0f)] private float courseEndTime = 5f;
        [Space]
        public UnityEvent OnStartCourse, OnStartHole;
        public UnityEvent OnStroke, OnFallOff, OnPlayerWaiting;
        public UnityEvent OnCompleteHole, OnCompleteCourse;

        private SwingController player;
        private HoleTile holeTile;
        private Course course;
        private Coroutine completeHoleRoutine;
        /// <summary>Array of position lists for each hole</summary>
        private List<Vector3>[] holePositions;
        private int holeIndex;

        public int[] Scores => holePositions.Select(positions => positions.Count()).ToArray();
        public int CurrentScore => holeIndex < holePositions.Length ? CurrentPositions.Count : 0;
        private List<Vector3> CurrentPositions => holePositions[holeIndex];

        protected override void Awake()
        {
            base.Awake();

            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");
            if (course == null && holeGenerator == null && GameManager.singleton == null) Debug.LogError($"{nameof(course)} is empty");

            PlayerHandler.OnSetPlayer.AddListener(ChangePlayer);
            PlayerHandler.OnPlayerReady.AddListener(BeginCourse);
            holeGenerator.OnGenerate.AddListener(UpdateHoleTile);
        }

        private void Start()
        {
            if (GameManager.singleton == null) Debug.LogWarning($"No {nameof(GameManager)} loaded");
            course = GameManager.singleton ? GameManager.singleton.SelectedCourse : new Course();
            holePositions = new List<Vector3>[course.Length];
            for (int i = 0; i < holePositions.Length; i++) holePositions[i] = new();

            GolfRoomManager.singleton.OnPlayerListChanged.AddListener(TryCompleteHole);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

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
            player = newPlayer;

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
            if (TryBeginHole()) OnStartCourse.Invoke();

            PlayerHandler.OnPlayerReady.RemoveListener(BeginCourse);
        }

        private bool TryBeginHole()
        {
            if (holeIndex >= course.Length) return false;

            holeGenerator.Generate(course.HoleData[holeIndex]);
            if (holeIndex > 0)
            {
                player.transform.SetPositionAndRotation(GolfRoomManager.singleton.GetHoleStartPosition(), Quaternion.identity);
                player.SetPhysicsEnabled(true);
                PlayerHandler.SwingControlsEnabled = true;
            }

            OnStartHole.Invoke();
            return true;
        }

        private void AddStroke()
        {
            holePositions[holeIndex].Add(player.transform.position);
            OnStroke.Invoke();
        }

        private void CheckFall()
        {
            if (player == null || player.Rigidbody.transform.position.y > ballMinY) return;

            player.transform.position = CurrentPositions[^1];
            player.Rigidbody.velocity = Vector3.zero;
            player.Rigidbody.angularVelocity = Vector3.zero;

            OnFallOff.Invoke();
        }

        private void HoleBall(SwingController ball)
        {
            var isPlayer = player == ball;
            if (isPlayer)
            {
                PlayerHandler.SwingControlsEnabled = false;
                ball.SetPhysicsEnabled(false);
            }

            if (CanChangeHoles()) CompleteHole();
            else if (isPlayer) OnPlayerWaiting.Invoke();
        }

        private void TryCompleteHole()
        {
            if (CanChangeHoles()) CompleteHole();
        }

        private bool CanChangeHoles() => holeTile && holeTile.BallCount == GolfRoomManager.singleton.roomSlots.Count;

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

            GolfRoomManager.singleton.EndGame();
        }
    }
}
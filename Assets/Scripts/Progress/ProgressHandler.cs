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
    public class ProgressHandler : Singleton<ProgressHandler>
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
        public UnityEvent OnStroke, OnFallOff;
        public UnityEvent OnCompleteHole, OnCompleteCourse;

        private SwingController player;
        private HoleTile holeTile;
        private Course course;
        /// <summary>Array of positions for each hole</summary>
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
        }

        private void ChangePlayer(SwingController oldPlayer, SwingController newPlayer)
        {
            player = newPlayer;

            if (oldPlayer) oldPlayer.OnSwing.RemoveListener(AddStroke);
            if (newPlayer) newPlayer.OnSwing.AddListener(AddStroke);
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
                PlayerHandler.SetControls(true);
            }

            OnStartHole.Invoke();
            return true;
        }

        private void UpdateHoleTile(HoleTile newHoleTile)
        {
            if (holeTile) holeTile.OnBallEnter.RemoveListener(HoleBall);

            holeTile = newHoleTile;
            if (holeTile) holeTile.OnBallEnter.AddListener(HoleBall);
        }

        private void AddStroke()
        {
            holePositions[holeIndex].Add(player.transform.position);
            OnStroke.Invoke();
        }

        private void HoleBall(SwingController ball)
        {
            if (player == ball)
            {
                PlayerHandler.SetControls(false, true);
                ball.SetPhysicsEnabled(false);
            }

            if (CanChangeHoles()) CompleteHole();
        }

        private bool CanChangeHoles() => holeTile.BallCount == GolfRoomManager.singleton.roomSlots.Count;

        private void CompleteHole() => _ = StartCoroutine(CompleteHoleRoutine());
        private IEnumerator CompleteHoleRoutine()
        {
            OnCompleteHole.Invoke();

            yield return new WaitForSeconds(holeEndTime);

            holeIndex++;
            if (!TryBeginHole()) yield return StartCoroutine(CompleteCourseRoutine());
        }

        private IEnumerator CompleteCourseRoutine()
        {
            OnCompleteCourse.Invoke();
            yield return new WaitForSeconds(Mathf.Max(courseEndTime - holeEndTime, 0f));

            switch (GolfRoomManager.singleton.mode)
            {
                case NetworkManagerMode.Host: GolfRoomManager.singleton.StopHost(); break;
                case NetworkManagerMode.ClientOnly: GolfRoomManager.singleton.StopClient(); break;
                case NetworkManagerMode.ServerOnly: GolfRoomManager.singleton.StopServer(); break;
            }
        }

        private void FixedUpdate()
        {
            CheckFall();
        }

        private void CheckFall()
        {
            if (player == null || player.Rigidbody.transform.position.y > ballMinY) return;

            player.transform.position = CurrentPositions[^1];
            player.Rigidbody.velocity = Vector3.zero;
            player.Rigidbody.angularVelocity = Vector3.zero;

            OnFallOff.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (PlayerHandler.singleton) PlayerHandler.OnSetPlayer.RemoveListener(ChangePlayer);
            if (PlayerHandler.Player) PlayerHandler.Player.OnSwing.RemoveListener(AddStroke);
            if (holeGenerator) holeGenerator.OnGenerate.RemoveListener(UpdateHoleTile);
            if (holeTile) holeTile.OnBallEnter.RemoveListener(HoleBall);
        }
    }
}
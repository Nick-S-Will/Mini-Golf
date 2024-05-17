using MiniGolf.Player;
using MiniGolf.Managers.Game;
using MiniGolf.Managers.SceneTransition;
using MiniGolf.Terrain;
using MiniGolf.Terrain.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System;

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
        public UnityEvent OnStartCourse;
        public UnityEvent OnStroke, OnFallOff;
        /// <summary>Passes amount of time before hole change</summary>
        public UnityEvent<float> OnCompleteHole;
        /// <summary>Passes amount of time before scene change</summary>
        public UnityEvent<float> OnCompleteCourse;

        private Rigidbody ballRigidbody;
        private HoleTile holeTile;
        private Course course;
        /// <summary>Array of positions for each hole</summary>
        private List<Vector3>[] holePositions;
        private int holeIndex, ballsInHole;

        private List<Vector3> CurrentPositions => holePositions[holeIndex];
        public int[] Scores => holePositions.Select(positions => positions.Count()).ToArray();
        public int CurrentScore => holeIndex < holePositions.Length ? CurrentPositions.Count : 0;
        public Func<int, bool> CanChangeHoles { get; set; } = ballsInHole => ballsInHole > 0;

        protected override void Awake()
        {
            base.Awake();

            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");
            if (course == null && holeGenerator == null && GameManager.singleton == null) Debug.LogError($"{nameof(course)} is empty");

            PlayerHandler.OnChangePlayer.AddListener(PlayerChanged);
            holeGenerator.OnGenerate.AddListener(UpdateHoleTile);
        }

        private void Start()
        {
            if (GameManager.singleton) course = GameManager.singleton.SelectedCourse;
            else if (holeGenerator) course = new Course("Test", new HoleData[] { holeGenerator.HoleData });
            holePositions = new List<Vector3>[course.Length];
            for (int i = 0; i < holePositions.Length; i++) holePositions[i] = new();

            if (TryBeginHole()) OnStartCourse.Invoke();
        }

        private void PlayerChanged(BallController oldPlayer, BallController newPlayer)
        {
            ballRigidbody = newPlayer ? newPlayer.GetComponent<Rigidbody>() : null;

            if (oldPlayer) oldPlayer.OnSwing.RemoveListener(AddStroke);
            if (newPlayer) newPlayer.OnSwing.AddListener(AddStroke);
        }

        private bool TryBeginHole()
        {
            if (holeIndex >= course.Length) return false;

            holeGenerator.Generate(course.HoleData[holeIndex]);
            if (holeIndex > 0)
            {
                ballRigidbody.transform.SetPositionAndRotation(PlayerHandler.singleton.transform.position, Quaternion.identity);
                ballsInHole = 0;
            }

            return true;
        }

        private void UpdateHoleTile(HoleTile holeTile)
        {
            if (this.holeTile) holeTile.OnBallEnter.RemoveListener(HoleBall);

            this.holeTile = holeTile;
            this.holeTile.OnBallEnter.AddListener(HoleBall);
        }

        private void AddStroke()
        {
            holePositions[holeIndex].Add(ballRigidbody.position);
            OnStroke.Invoke();
        }

        private void HoleBall(BallController ball)
        {
            if (ballRigidbody.gameObject == ball.gameObject)
            {
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
            }

            ballsInHole++;

            if (CanChangeHoles(ballsInHole)) CompleteHole();
        }

        private void CompleteHole() => _ = StartCoroutine(CompleteHoleRoutine());
        private IEnumerator CompleteHoleRoutine()
        {
            holeIndex++;
            OnCompleteHole.Invoke(holeEndTime);
            yield return new WaitForSeconds(holeEndTime);

            if (!TryBeginHole()) yield return StartCoroutine(CompleteCourseRoutine());
        }

        private IEnumerator CompleteCourseRoutine()
        {
            OnCompleteCourse.Invoke(courseEndTime);
            yield return new WaitForSeconds(courseEndTime);

            SceneTransitionManager.ChangeScene(Scene.Title);
        }

        private void FixedUpdate()
        {
            CheckFall();
        }

        private void CheckFall()
        {
            if (ballRigidbody == null || ballRigidbody.transform.position.y > ballMinY) return;

            ballRigidbody.transform.position = CurrentPositions[^1];
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;

            OnFallOff.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (PlayerHandler.singleton) PlayerHandler.OnChangePlayer.RemoveListener(PlayerChanged);
            if (PlayerHandler.Player) PlayerHandler.Player.OnSwing.RemoveListener(AddStroke);
            if (holeGenerator) holeGenerator.OnGenerate.RemoveListener(UpdateHoleTile);
            if (holeTile) holeTile.OnBallEnter.RemoveListener(HoleBall);
        }
    }
}
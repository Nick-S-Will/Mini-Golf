using MiniGolf.Controls;
using MiniGolf.Managers.Game;
using MiniGolf.Managers.SceneTransition;
using MiniGolf.Terrain;
using MiniGolf.Terrain.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Progress
{
    public class ProgressHandler : MonoBehaviour
    {
        [SerializeField] private BallController ballController;
        [SerializeField] private float ballMinY = -10f;
        [Space]
        [SerializeField] private HoleGenerator holeGenerator;
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
        private int holeIndex;

        private List<Vector3> CurrentPositions => holePositions[holeIndex];
        public int[] Scores => holePositions.Select(positions => positions.Count()).ToArray();
        public int CurrentScore => holeIndex < holePositions.Length ? CurrentPositions.Count : 0;

        private void Awake()
        {
            if (ballController == null) Debug.LogError($"{nameof(ballController)} not assigned");
            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");
            if (course == null && holeGenerator == null && GameManager.instance == null) Debug.LogError($"{nameof(course)} is empty");
            
            ballRigidbody = ballController.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ballController.OnSwing.AddListener(Stroke);
            holeGenerator.OnGenerate.AddListener(UpdateHoleTile);

            if (GameManager.instance) course = GameManager.instance.SelectedCourse;
            else if (holeGenerator) course = new Course("Test", new HoleData[] { holeGenerator.HoleData });
            holePositions = new List<Vector3>[course.Length];
            for (int i = 0; i < holePositions.Length; i++) holePositions[i] = new();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (TryBeginHole()) OnStartCourse.Invoke();
        }

        private bool TryBeginHole()
        {
            if (holeIndex >= course.Length) return false;

            holeGenerator.Generate(course.HoleData[holeIndex]);
            if (holeIndex > 0) ballController.transform.SetPositionAndRotation(holePositions[0][0], Quaternion.identity);

            return true;
        }

        private void UpdateHoleTile(HoleTile holeTile)
        {
            if (this.holeTile) holeTile.OnBallEnter.RemoveListener(CompleteHole);

            this.holeTile = holeTile;
            this.holeTile.OnBallEnter.AddListener(CompleteHole);
        }

        private void Stroke()
        {
            holePositions[holeIndex].Add(ballController.transform.position);
            OnStroke.Invoke();
        }

        private void CompleteHole() => _ = StartCoroutine(CompleteHoleRoutine());
        private IEnumerator CompleteHoleRoutine()
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;

            holeIndex++;
            OnCompleteHole.Invoke(holeEndTime);
            yield return new WaitForSeconds(holeEndTime);

            if (!TryBeginHole()) _ = StartCoroutine(CompleteCourseRoutine());
        }

        private IEnumerator CompleteCourseRoutine()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

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
            if (ballController.transform.position.y < ballMinY)
            {
                ballController.transform.position = CurrentPositions[^1];
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;

                OnFallOff.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (ballController) ballController.OnSwing.AddListener(Stroke);
            if (holeGenerator) holeGenerator.OnGenerate.AddListener(UpdateHoleTile);
            if (holeTile) holeTile.OnBallEnter.RemoveListener(CompleteHole);
        }
    }
}
using MiniGolf.Controls;
using MiniGolf.Managers.Game;
using MiniGolf.Managers.SceneTransition;
using MiniGolf.Terrain;
using MiniGolf.Terrain.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Managers.Progress
{
    public class ProgressManager : MonoBehaviour
    {
        [SerializeField] private BallController ballController;
        [SerializeField] private float ballMinY = -10f;
        [Space]
        [SerializeField] private HoleGenerator holeGenerator;
        [SerializeField] private Course course;
        [Space]
        public UnityEvent OnStroke;
        public UnityEvent OnCompleteHole;

        private Rigidbody ballRigidbody;
        private HoleTile holeTile;
        /// <summary>Array of positions for each hole</summary>
        private List<Vector3>[] holePositions;
        private int holeIndex;

        private List<Vector3> CurrentPositions => holePositions[holeIndex];
        public int CurrentScore => holeIndex < holePositions.Length ? CurrentPositions.Count : 0;

        private void Awake()
        {
            if (ballController == null) Debug.LogError($"{nameof(ballController)} not assigned");
            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");
            if ((course == null || course.HoleData.Length == 0) && GameManager.instance == null) Debug.LogError($"{nameof(course)} is empty");
            
            ballRigidbody = ballController.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ballController.OnSwing.AddListener(Stroke);
            holeGenerator.OnGenerate.AddListener(UpdateHoleTile);

            if (GameManager.instance) course = GameManager.instance.SelectedCourse;
            holePositions = new List<Vector3>[course.Length];
            for (int i = 0; i < holePositions.Length; i++) holePositions[i] = new();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _ = TryBeginHole();
        }

        private bool TryBeginHole()
        {
            if (holeIndex >= course.Length) return false;

            holeGenerator.Generate(course.HoleData[holeIndex]);
            if (holeIndex > 0) ballController.transform.SetPositionAndRotation(holePositions[0][0], Quaternion.identity);

            return true;
        }

        private void UpdateHoleTile()
        {
            if (holeTile) holeTile.OnBallEnter.RemoveListener(CompleteHole);

            holeTile = holeGenerator.HoleTile;
            holeTile.OnBallEnter.AddListener(CompleteHole);
        }

        private void Stroke()
        {
            holePositions[holeIndex].Add(ballController.transform.position);
            OnStroke.Invoke();
        }

        private void CompleteHole()
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;

            holeIndex++;
            OnCompleteHole.Invoke();

            if (!TryBeginHole())
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                SceneTransitionManager.instance.ChangeScene(Scene.Title);
            }
        }

        private void FixedUpdate()
        {
            CheckFall();
        }

        private void CheckFall()
        {
            if (ballController.transform.position.y < ballMinY)
            {
                var lastPosition = CurrentPositions[^1];
                ballController.transform.position = lastPosition;
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
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
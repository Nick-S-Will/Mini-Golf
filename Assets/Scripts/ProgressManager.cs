using MiniGolf.Controls;
using MiniGolf.Terrain;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Progress
{
    public class ProgressManager : MonoBehaviour
    {
        [SerializeField] private BallController ballController;
        [SerializeField] private float ballMinY = -10f;
        [Space]
        [SerializeField] private CourseGenerator courseGenerator;
        [SerializeField] GenerationSettings[] courseSettings = new GenerationSettings[] { GenerationSettings.Default };
        [Space]
        public UnityEvent OnStroke;
        public UnityEvent OnCompleteCourse;

        private Rigidbody ballRigidbody;
        private HoleTile holeTile;
        /// <summary>Array of positions for each course</summary>
        private List<Vector3>[] coursePositions;
        private int courseIndex;

        private List<Vector3> CurrentPositions => coursePositions[courseIndex];
        public int CurrentScore => courseIndex < coursePositions.Length ? CurrentPositions.Count : 0;

        private void Awake()
        {
            if (ballController == null) Debug.LogError($"{nameof(ballController)} not assigned");
            if (courseGenerator == null) Debug.LogError($"{nameof(courseGenerator)} not assigned");
            if (courseSettings.Length == 0) Debug.LogError($"{nameof(courseSettings)} is empty");
            
            ballRigidbody = ballController.GetComponent<Rigidbody>();
            coursePositions = new List<Vector3>[courseSettings.Length];
            for (int i = 0; i < coursePositions.Length; i++) coursePositions[i] = new List<Vector3>();
        }

        private void Start()
        {
            ballController.OnSwing.AddListener(Stroke);
            courseGenerator.OnGenerate.AddListener(UpdateHoleTile);

            _ = TryBeginCourse();
        }

        private bool TryBeginCourse()
        {
            if (courseIndex >= courseSettings.Length) return false;

            courseGenerator.Generate(courseSettings[courseIndex]);
            if (courseIndex > 0) ballController.transform.SetPositionAndRotation(coursePositions[0][0], Quaternion.identity);

            return true;
        }

        private void UpdateHoleTile()
        {
            if (holeTile) holeTile.OnBallEnter.RemoveListener(CompleteCourse);

            holeTile = courseGenerator.HoleTile;
            holeTile.OnBallEnter.AddListener(CompleteCourse);
        }

        private void Stroke()
        {
            coursePositions[courseIndex].Add(ballController.transform.position);
            OnStroke.Invoke();
        }

        private void CompleteCourse()
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;

            courseIndex++;
            OnCompleteCourse.Invoke();

            if (!TryBeginCourse())
            {
                var scores = new StringBuilder("Scores: ");
                foreach (var positions in coursePositions) scores.Append($"{positions.Count}, ");
                Debug.Log(scores); // TODO: Round end actions
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
            if (courseGenerator) courseGenerator.OnGenerate.AddListener(UpdateHoleTile);
            if (holeTile) holeTile.OnBallEnter.RemoveListener(CompleteCourse);
        }
    }
}
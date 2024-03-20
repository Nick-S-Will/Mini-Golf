using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody))]
    public class ClubController : SwingController
    {
        [SerializeField][Range(10f, 270f)] private float maxBackswingAngle = 90f;
        [SerializeField][Min(0f)] private float swingAngularVelocityTolerance = 0.5f;
        [Space]
        [SerializeField] private GameObject graphicsParent;
        [SerializeField] private BallController ballController;

        private Coroutine swingRoutine;
        private Vector3 positionFromBall;

        public bool IsSwinging => swingRoutine != null;
        public override bool CanBackswing => !ballController.IsMoving;

        protected override void Awake()
        {
            base.Awake();

            if (graphicsParent == null) Debug.LogError($"{nameof(graphicsParent)} not assigned");
            if (ballController == null) Debug.LogError($"{nameof(ballController)} not assigned");
            else positionFromBall = transform.position - ballController.transform.position;
        }
        
        protected override void Start()
        {
            base.Start();

            OnBackswing.AddListener(MoveToRelativePositionFromBall);
            OnBackswingChange.AddListener(UpdateClubAngle);
        }

        private void MoveToRelativePositionFromBall()
        {
            graphicsParent.SetActive(true);

            var projectedRotation = Quaternion.Euler(0f, camTransform.rotation.eulerAngles.y, 0f);
            var position = ballController.transform.position + projectedRotation * positionFromBall;
            transform.position = position;

            transform.forward = Vector3.ProjectOnPlane(ballController.transform.position - transform.position, Vector3.up);
        }
        
        private void UpdateClubAngle()
        {
            var angles = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(BackswingScaler * maxBackswingAngle, angles.y, angles.z);
        }

        protected override void CancelBackswing()
        {
            graphicsParent.SetActive(false);
        }

        protected override void Swing()
        {
            swingRoutine ??= StartCoroutine(SwingRoutine());
        }
        private IEnumerator SwingRoutine() // TODO: Improve visual
        {
            while (transform.eulerAngles.x <= maxBackswingAngle)
            {
                Rigidbody.AddTorque(MaxStrokeStrength * Time.fixedDeltaTime * -transform.right);

                yield return new WaitForFixedUpdate();
            }

            while (Rigidbody.angularVelocity.magnitude >= swingAngularVelocityTolerance)
            {
                Rigidbody.AddTorque(Time.fixedDeltaTime * -Rigidbody.angularVelocity);

                yield return new WaitForFixedUpdate();
            }
            Rigidbody.angularVelocity = Vector3.zero;

            graphicsParent.SetActive(false);

            swingRoutine = null;
        }
    }
}
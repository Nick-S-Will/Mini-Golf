using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Controls
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallController : SwingController
    {
        [Space]
        [SerializeField][Min(0f)] private float ballVelocityTolerance = 0.01f;
        [Space]
        public UnityEvent OnStopMoving;

        private Vector3 lastVelocity;

        public bool IsMoving => Rigidbody.velocity.magnitude > ballVelocityTolerance;
        public override bool CanBackswing => !IsMoving;

        protected override void Start()
        {
            base.Start();
        }

        private void FixedUpdate()
        {
            UpdateMovingStatus();
        }

        private void UpdateMovingStatus()
        {
            var wasMoving = lastVelocity.magnitude > ballVelocityTolerance;
            if (wasMoving && !IsMoving)
            {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
                OnStopMoving.Invoke();
            }

            lastVelocity = Rigidbody.velocity;
        }

        protected override void Swing()
        {
            if (BackswingScaler == 0f) return;

            var strokeStrength = BackswingScaler * MaxStrokeStrength;
            var strokeDirection = Vector3.ProjectOnPlane(CamTransform.forward, Vector3.up);

            Rigidbody.AddForce(strokeStrength * strokeDirection, ForceMode.Impulse);
        }
    }
}
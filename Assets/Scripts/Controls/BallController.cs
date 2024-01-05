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
        public UnityEvent OnStopMoving; // TODO: Make controls unavailable while ball is moving

        private Vector3 lastVelocity;

        public bool IsMoving => Rigidbody.velocity.magnitude > ballVelocityTolerance;

        protected override void Start()
        {
            base.Start();
        }

        private void FixedUpdate()
        {
            var wasMoving = lastVelocity.magnitude > ballVelocityTolerance;
            if (wasMoving && !IsMoving) OnStopMoving.Invoke();

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
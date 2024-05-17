using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class BallController : SwingController
    {
        [Space]
        [SerializeField][Min(0f)] private float ballVelocityTolerance = 0.01f;
        [SerializeField][Min(0f)] private float ballSurfaceAngleTolerance = 1f;
        [Space]
        public UnityEvent OnStopMoving;

        private SphereCollider sphereCollider;
        private Vector3 lastVelocity;

        public bool IsMoving => Rigidbody.velocity.magnitude > ballVelocityTolerance;
        public override bool CanBackswing => !IsMoving;

        protected override void Awake()
        { 
            base.Awake();

            sphereCollider = GetComponent<SphereCollider>();
        }

        protected override void Start() => base.Start();
        
        private void FixedUpdate()
        {
            UpdateMovingStatus();
        }

        private void UpdateMovingStatus()
        {
            var wasMoving = lastVelocity.magnitude > ballVelocityTolerance;
            if (wasMoving && !IsMoving && OnFlatSurface())
            {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
                OnStopMoving.Invoke();
            }

            lastVelocity = Rigidbody.velocity;
        }

        private bool OnFlatSurface()
        {
            var distance = sphereCollider.radius + ballVelocityTolerance;
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, distance)) return false;

            var angle = Vector3.Angle(Vector3.up, hitInfo.normal);
            return angle < ballSurfaceAngleTolerance;
        }

        protected override void Swing()
        {
            var strokeStrength = BackswingScaler * MaxStrokeStrength;
            var strokeDirection = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);

            Rigidbody.AddForce(strokeStrength * strokeDirection, ForceMode.Impulse);
        }
    }
}
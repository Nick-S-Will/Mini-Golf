using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class SwingController : MonoBehaviour
    {
        public static UnityEvent<SwingController> OnStartPlayer = new();

        [Header("Move Detection")]
        [SerializeField] private SphereCollider sphereCollider;
        [SerializeField][Min(0f)] private float ballVelocityTolerance = 0.01f;
        [SerializeField][Min(0f)] private float ballSurfaceAngleTolerance = 1f;
        [Header("Swing Settings")]
        [SerializeField][Min(0f)] private float swingInputSensitivity = 0.1f;
        [SerializeField][Min(0f)] private float maxStrokeStrength = 1f;
        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is started</summary>
        [Space]
        public UnityEvent OnBackswing;
        /// <summary>Invoked when <see cref="BackswingScaler"/> changes during <see cref="Backswinging"/></summary>
        public UnityEvent OnBackswingChange;
        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is canceled and <see cref="BackswingScaler"/> = 0</summary>
        public UnityEvent OnBackswingCancel;
        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is canceled and <see cref="BackswingScaler"/> > 0</summary>
        public UnityEvent OnSwing;
        public UnityEvent OnStopMoving;

        private Vector3 lastVelocity;

        protected Transform CameraTransform { get; private set; }
        protected float SwingInputSensitivity => swingInputSensitivity;
        protected float MaxStrokeStrength => maxStrokeStrength;

        public Rigidbody Rigidbody { get; private set; }
        public float BackswingScaler { get; private set; }
        public bool IsBackswinging { get; private set; }
        public bool IsMoving => Rigidbody.velocity.magnitude > ballVelocityTolerance;

        protected virtual void Awake()
        {
            CameraTransform = Camera.main.transform;
            Rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            OnSwing.AddListener(Swing);
            OnBackswingCancel.AddListener(CancelBackswing);

            OnStartPlayer.Invoke(this);
        }

        protected virtual void FixedUpdate()
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

        public void SetPhysicsEnabled(bool enabled)
        {
            if (!enabled && !Rigidbody.isKinematic) Rigidbody.velocity = Vector3.zero;

            Rigidbody.useGravity = enabled;
            GetComponent<Collider>().enabled = enabled;
        }

        public void ToggleBackswing(InputAction.CallbackContext context)
        {
            if (IsMoving) return;

            if (context.started)
            {
                OnBackswing.Invoke();
                IsBackswinging = true;
            }
            else if (context.canceled)
            {
                if (BackswingScaler > 0f)
                {
                    OnSwing.Invoke();
                    BackswingScaler = 0f;
                }
                else OnBackswingCancel.Invoke();
                IsBackswinging = false;
            }
        }

        public void Backswinging(InputAction.CallbackContext context)
        {
            if (!IsBackswinging) return;

            BackswingScaler = Mathf.Clamp(BackswingScaler - swingInputSensitivity * context.ReadValue<float>() * Time.deltaTime, 0f, 1f);

            OnBackswingChange.Invoke();
        }

        protected virtual void CancelBackswing() { }
        protected abstract void Swing();
    }
}
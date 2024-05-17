using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    public abstract class SwingController : NetworkBehaviour
    {
        [Space]
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

        protected Transform CameraTransform { get; private set; }
        protected Rigidbody Rigidbody { get; private set; }
        protected float SwingInputSensitivity => swingInputSensitivity;
        protected float MaxStrokeStrength => maxStrokeStrength;

        public float BackswingScaler { get; private set; }
        public bool IsBackswinging { get; private set; }
        public abstract bool CanBackswing { get; }

        protected virtual void Awake()
        {
            CameraTransform = UnityEngine.Camera.main.transform;
            Rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnStartAuthority()
        {
            enabled = true;
        }

        public override void OnStopAuthority()
        {
            enabled = false;
        }

        protected virtual void Start()
        {
            OnSwing.AddListener(Swing);
            OnBackswingCancel.AddListener(CancelBackswing);
        }

        public void ToggleBackswing(InputAction.CallbackContext context)
        {
            if (!enabled || !CanBackswing) return;

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
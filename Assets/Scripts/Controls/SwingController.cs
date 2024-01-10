using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Controls
{
    public abstract class SwingController : MonoBehaviour
    {
        [SerializeField] private Transform camTransform;
        [Space]
        [SerializeField][Min(0f)] private float swingInputSensitivity = 0.1f;
        [SerializeField][Min(0f)] private float maxStrokeStrength = 1f;
        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is started</summary>
        [Space]
        public UnityEvent OnBackswing;
        /// <summary>Invoked when <see cref="BackswingScaler"/> changes during <see cref="Backswinging"/></summary>
        public UnityEvent OnBackswingChange;
        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is canceled</summary>
        public UnityEvent OnSwing;

        protected Rigidbody Rigidbody { get; private set; }
        protected Transform CamTransform => camTransform;
        protected float SwingInputSensitivity => swingInputSensitivity;
        protected float MaxStrokeStrength => maxStrokeStrength;

        public float BackswingScaler { get; private set; }
        public bool IsBackswinging { get; private set; }
        public abstract bool CanBackswing { get; }

        protected virtual void Start()
        {
            if (camTransform == null) Debug.LogError($"{nameof(camTransform)} not assigned");

            Rigidbody = GetComponent<Rigidbody>();

            OnSwing.AddListener(Swing);
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
                OnSwing.Invoke();
                BackswingScaler = 0f;
                IsBackswinging = false;
            }
        }

        public void Backswinging(InputAction.CallbackContext context)
        {
            if (!IsBackswinging) return;

            BackswingScaler = Mathf.Clamp(BackswingScaler - swingInputSensitivity * context.ReadValue<float>() * Time.deltaTime, 0f, 1f);

            OnBackswingChange.Invoke();
        }

        protected abstract void Swing();
    }
}
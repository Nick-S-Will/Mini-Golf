using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Controls
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallController : MonoBehaviour
    {
        [SerializeField][Min(0f)] private float swingInputSensitivity = 0.1f, maxStrokeStrength = 1f;
        [Space]
        [SerializeField] private Transform camTransform;

        private new Rigidbody rigidbody;

        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is started</summary>
        [HideInInspector] public UnityEvent OnBackswing;
        /// <summary>Invoked when <see cref="BackswingScaler"/> changes during <see cref="Backswinging"/></summary>
        [HideInInspector] public UnityEvent OnBackswingChange;
        /// <summary>Invoked when <see cref="ToggleBackswing(InputAction.CallbackContext)"/>'s context is canceled</summary>
        [HideInInspector] public UnityEvent OnSwing;
        public float BackswingScaler { get; private set; }
        public bool IsBackswinging { get; private set; }

        private void Start()
        {
            if (camTransform == null) Debug.LogError($"{nameof(camTransform)} not assigned");

            rigidbody = GetComponent<Rigidbody>();
            
            OnSwing.AddListener(Swing);
        }

        public void ToggleBackswing(InputAction.CallbackContext context)
        {
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

            BackswingScaler = Mathf.Clamp(BackswingScaler - swingInputSensitivity * context.ReadValue<float>(), 0f, 1f);

            OnBackswingChange.Invoke();
        }

        private void Swing()
        {
            var strokeStrength = BackswingScaler * maxStrokeStrength;
            var strokeDirection = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up);

            rigidbody.AddForce(strokeStrength * strokeDirection, ForceMode.Impulse);
        }
    }
}
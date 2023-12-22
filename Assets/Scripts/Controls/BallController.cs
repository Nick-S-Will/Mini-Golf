using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Controls
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallController : MonoBehaviour
    {
        [SerializeField][Min(0f)] private float maxInputSwingDelta = 100f, maxStrokeStrength = 5f;
        [Space]
        [SerializeField] private Transform camTransform;
        /// <summary>Invoked every time the swing delta changes during swing and passes the <see cref="SwingScaler"/></summary>
        [Space]
        [SerializeField] private UnityEvent<float> ChargingSwing;
        /// <summary>Invoked when swing action is canceled and passes the <see cref="SwingScaler"/></summary>
        [SerializeField] private UnityEvent<float> OnEndSwing;

        private new Rigidbody rigidbody;
        private float inputSwingDelta;
        private bool isSwinging;

        /// <summary>Calculates the swing scale [0, 1] from input while <see cref="IsSwinging"/></summary>
        public float SwingScaler => Mathf.Clamp(inputSwingDelta, 0f, maxInputSwingDelta) / maxInputSwingDelta;
        public bool IsSwinging => isSwinging;

        private void Start()
        {
            if (camTransform == null) Debug.LogError($"{nameof(camTransform)} not assigned");

            rigidbody = GetComponent<Rigidbody>();
            
            OnEndSwing.AddListener(StrokeBall);
        }

        public void ToggleSwing(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isSwinging = true;
            }
            else if (context.canceled)
            {
                OnEndSwing?.Invoke(SwingScaler);
                inputSwingDelta = 0f;
                isSwinging = false;
            }
        }

        public void Swinging(InputAction.CallbackContext context)
        {
            if (!isSwinging) return;

            inputSwingDelta -= context.ReadValue<float>();

            ChargingSwing?.Invoke(SwingScaler);
        }

        private void StrokeBall(float scale)
        {
            var strokeStrength = scale * maxStrokeStrength;
            var strokeDirection = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up);

            rigidbody.AddForce(strokeStrength * strokeDirection, ForceMode.Impulse);
        }
    }
}
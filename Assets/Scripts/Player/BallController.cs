using UnityEngine;

namespace MiniGolf.Player
{
    public class BallController : SwingController
    {
        protected override void Awake() => base.Awake();
        
        protected override void Start() => base.Start();

        protected override void FixedUpdate() => base.FixedUpdate();
        
        protected override void Swing()
        {
            var strokeStrength = BackswingScaler * MaxStrokeStrength;
            var strokeDirection = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up);

            Rigidbody.AddForce(strokeStrength * strokeDirection, ForceMode.Impulse);
        }
    }
}
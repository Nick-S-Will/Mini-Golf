using UnityEngine;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpinningObstacle : MonoBehaviour
    {
        [SerializeField] private Vector3 angularVelocity;

        private new Rigidbody rigidbody;
        private Quaternion lastRotation;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            lastRotation = transform.rotation;
            angularVelocity = lastRotation * angularVelocity;
        }

        private void FixedUpdate()
        {
            var rotation = (lastRotation * Quaternion.Euler(Time.fixedDeltaTime * angularVelocity)).normalized;
            rigidbody.MoveRotation(rotation);

            lastRotation = rotation;
        }
    }
}
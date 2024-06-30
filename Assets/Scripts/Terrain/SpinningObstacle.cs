using UnityEngine;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpinningObstacle : MonoBehaviour
    {
        [SerializeField] private Vector3 angularVelocity;

        private new Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddTorque(Mathf.Deg2Rad * angularVelocity, ForceMode.VelocityChange);
        }
    }
}
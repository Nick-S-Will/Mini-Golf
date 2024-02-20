using UnityEngine;


namespace MiniGolf.Camera
{
    public class TransitionTarget : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private bool hideOnTransitionAway = true;
        [Space]
        [SerializeField] private Color gizmoColor = Color.white;

        public GameObject Target => target;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public bool HideOnTransitionAway => hideOnTransitionAway;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}
using System.Collections;
using UnityEngine;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(SphereCollider))]
    public class Flag : MonoBehaviour
    {
        [SerializeField] private Transform meshParent;
        [SerializeField] private Vector3 moveDelta = Vector3.up;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private string playerTag = "Player";

        private Coroutine moveRoutine;
        private Transform target;
        private SphereCollider sphereCollider;
        private float MaxDistance => sphereCollider.radius;

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }

        private IEnumerator MoveRoutine()
        {
            var startPosition = meshParent.position;
            var endPosition = startPosition + meshParent.rotation * moveDelta;

            while (target != null)
            {
                var distance = Vector3.Distance(transform.position, target.position);
                var interpolate = (distance - minDistance) / (MaxDistance - minDistance);
                var position = Vector3.Lerp(endPosition, startPosition, interpolate);
                meshParent.position = position;

                yield return null;
            }

            meshParent.position = startPosition;
            moveRoutine = null;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(playerTag) && moveRoutine == null)
            {
                target = collider.transform;
                moveRoutine = StartCoroutine(MoveRoutine());
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.transform == target) target = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, minDistance);
        }
    }
}
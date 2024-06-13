using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private List<Transform> targets = new();
        private SphereCollider sphereCollider;
        private float MaxDistance => sphereCollider.radius;

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(playerTag))
            {
                targets.Add(collider.transform);
                StartMoving();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            _ = targets.Remove(collider.transform);
        }

        private void StartMoving() => moveRoutine ??= StartCoroutine(MoveRoutine());
        private IEnumerator MoveRoutine()
        {
            var startPosition = meshParent.position;
            var endPosition = startPosition + meshParent.rotation * moveDelta;

            while (this && targets.Count > 0)
            {
                var distance = targets.Min(target => Vector3.Distance(transform.position, target.position));
                var interpolate = (distance - minDistance) / (MaxDistance - minDistance);
                var position = Vector3.Lerp(endPosition, startPosition, interpolate);
                meshParent.position = position;

                yield return null;

                targets.RemoveAll(target => target == null);
            }

            meshParent.position = startPosition;
            moveRoutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, minDistance);
        }
    }
}
using System.Collections;
using UnityEngine;

namespace MiniGolf.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraTransitioner : MonoBehaviour
    {
        [SerializeField][Min(0f)] private float transitionTime = 1.0f;
        [Space]
        [SerializeField] private TransitionTarget startingTarget;

        private new UnityEngine.Camera camera;
        private TransitionTarget currentTarget;
        private Coroutine transitionRoutine;

        public float TransitionTime
        {
            get => transitionTime;
            set => transitionTime = value >= 0f ? value : transitionTime;
        }

        private void Awake()
        {
            camera = GetComponent<UnityEngine.Camera>();
            if (camera == null)
            {
                Debug.LogError($"{nameof(CameraTransitioner)} must be placed on object with a {nameof(UnityEngine.Camera)}");
                return;
            }

            if (startingTarget) GoToTarget(startingTarget);
        }

        private IEnumerator GoToTargetRoutine(TransitionTarget target)
        {
            if (currentTarget && currentTarget.HideOnTransitionAway)
            {
                currentTarget.Target.SetActive(false);
            }

            var distance = Vector3.Distance(camera.transform.position, target.Position);
            var moveSpeed = transitionTime == 0 ? float.MaxValue : distance / transitionTime;

            var angle = Quaternion.Angle(camera.transform.rotation, target.Rotation);
            var angularSpeed = transitionTime == 0 ? float.MaxValue : angle / transitionTime;

            while (camera.transform.position != target.Position)
            {
                var maxDistance = moveSpeed * Time.deltaTime;
                var position = Vector3.MoveTowards(camera.transform.position, target.Position, maxDistance);

                var maxAngle = angularSpeed * Time.deltaTime;
                var rotation = Quaternion.RotateTowards(camera.transform.rotation, target.Rotation, maxAngle);

                camera.transform.SetPositionAndRotation(position, rotation);

                yield return null;
            }

            target.Target.SetActive(true);
            currentTarget = target;
            transitionRoutine = null;
        }

        public void GoToTarget(TransitionTarget target)
        {
            if (transitionRoutine != null) StopCoroutine(transitionRoutine);

            transitionRoutine = StartCoroutine(GoToTargetRoutine(target));
        }

        [ContextMenu("Go To Starting Target")]
        private void GoToStartingTarget()
        {
            GetComponent<UnityEngine.Camera>().transform.SetPositionAndRotation(startingTarget.Position, startingTarget.Rotation);
        }

        public void Exit() => Application.Quit();
    }
}
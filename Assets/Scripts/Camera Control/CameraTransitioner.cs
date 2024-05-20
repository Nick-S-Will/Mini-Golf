using System.Collections;
using System.Linq;
using UnityEngine;

namespace MiniGolf.CameraControl
{
    [RequireComponent(typeof(Camera))]
    public class CameraTransitioner : MonoBehaviour
    {
        [SerializeField][Min(0f)] private float transitionTime = 1.0f;
        [Space]
        [SerializeField] private TransitionTarget startingTarget;

        private Camera cam;
        private TransitionTarget currentTarget;
        private Coroutine transitionRoutine;

        public float TransitionTime
        {
            get => transitionTime;
            set => transitionTime = value >= 0f ? value : transitionTime;
        }

        private void Awake()
        {
            cam = GetComponent<Camera>();
            currentTarget = FindObjectsOfType<TransitionTarget>().First(target => target.Target.activeSelf);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start()
        {
            if (startingTarget) GoToTarget(startingTarget);
        }

        public void GoToTarget(TransitionTarget target)
        {
            if (transitionRoutine != null) StopCoroutine(transitionRoutine);

            transitionRoutine = StartCoroutine(GoToTargetRoutine(target));
        }

        private IEnumerator GoToTargetRoutine(TransitionTarget target)
        {
            if (currentTarget && currentTarget.HideOnTransitionAway)
            {
                currentTarget.Target.SetActive(false);
            }

            var distance = Vector3.Distance(cam.transform.position, target.Position);
            var moveSpeed = transitionTime == 0 ? float.MaxValue : distance / transitionTime;

            var angle = Quaternion.Angle(cam.transform.rotation, target.Rotation);
            var angularSpeed = transitionTime == 0 ? float.MaxValue : angle / transitionTime;

            while (cam.transform.position != target.Position)
            {
                var maxDistance = moveSpeed * Time.deltaTime;
                var position = Vector3.MoveTowards(cam.transform.position, target.Position, maxDistance);

                var maxAngle = angularSpeed * Time.deltaTime;
                var rotation = Quaternion.RotateTowards(cam.transform.rotation, target.Rotation, maxAngle);

                cam.transform.SetPositionAndRotation(position, rotation);

                yield return null;
            }

            target.Target.SetActive(true);
            currentTarget = target;
            transitionRoutine = null;
        }

        [ContextMenu("Go To Starting Target")]
        private void GoToStartingTarget()
        {
            GetComponent<Camera>().transform.SetPositionAndRotation(startingTarget.Position, startingTarget.Rotation);
        }

        public void Exit() => Application.Quit();
    }
}
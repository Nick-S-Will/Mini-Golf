using Cinemachine;
using MiniGolf.Player;
using MiniGolf.Progress;
using System.Collections;
using UnityEngine;

namespace MiniGolf.CameraControl
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class HoleViewer : MonoBehaviour
    {
        [SerializeField] private ProgressHandler progressHandler;
        [SerializeField] private FreeLookTargeter freeLookTargeter;
        [SerializeField] private Transform holeBoundsCenter;
        [Space]
        [SerializeField][Min(0f)] private float cameraBoundsMargin = 3f;
        [SerializeField][Min(0f)] private float viewDelay = 2f, viewTime = 5f;

        private CinemachineVirtualCamera virtualCamera;
        private Coroutine viewHoleRoutine;

        private void Awake()
        {
            if (progressHandler == null) Debug.LogError($"{nameof(progressHandler)} not assigned");
            if (freeLookTargeter == null) Debug.LogError($"{nameof(freeLookTargeter)} not assigned");
            if (holeBoundsCenter == null) Debug.LogError($"{nameof(holeBoundsCenter)} not assigned");

            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.enabled = false;
        }

        private void OnEnable()
        {
            progressHandler.OnStartHole.AddListener(ViewHole);
        }

        private void OnDisable()
        {
            if (progressHandler) progressHandler.OnStartHole.RemoveListener(ViewHole);
        }

        private void ViewHole()
        {
            if (viewTime == 0f) return;

            viewHoleRoutine ??= StartCoroutine(ViewHoleRoutine());
        }
        private IEnumerator ViewHoleRoutine()
        {
            PlayerHandler.SetControls(false);

            var bounds = progressHandler.HoleGenerator.HoleBounds;
            var boundsMaxExtent = Mathf.Max(bounds.extents.x, bounds.extents.z);
            var camZOffset = boundsMaxExtent + cameraBoundsMargin;
            holeBoundsCenter.position = bounds.center;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -camZOffset;
            virtualCamera.enabled = true;

            yield return new WaitForSeconds(viewDelay);

            float animationCompletion = 0f, animateSpeed = 1f / viewTime;
            while (animationCompletion < 1f)
            {
                var angle = Mathf.Lerp(0f, 360f, animationCompletion);
                holeBoundsCenter.rotation = Quaternion.Euler(0f, angle, 0f);

                animationCompletion += animateSpeed * Time.deltaTime;

                yield return null;
            }

            holeBoundsCenter.position = Vector3.zero;
            virtualCamera.enabled = false;
            PlayerHandler.SetControls(true);

            freeLookTargeter.LookAtNewTarget(progressHandler.HoleGenerator.CurrentHoleTile);

            viewHoleRoutine = null;
        }
    }
}
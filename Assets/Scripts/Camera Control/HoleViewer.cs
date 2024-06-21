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
        [SerializeField] private Transform holeBoundsCenter;
        [Space]
        [SerializeField][Min(0f)] private float cameraBoundsMargin = 3f;
        [SerializeField][Min(0f)] private float viewDelay = 2f, viewTime = 5f;

        private CinemachineVirtualCamera virtualCamera;
        private Coroutine viewHoleRoutine;

        public float ViewTime
        {
            get => viewTime;
            set => viewTime = value;
        }

        private void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.enabled = false;

            if (progressHandler == null) Debug.LogError($"{nameof(progressHandler)} not assigned");
        }

        private void OnEnable()
        {
            progressHandler.OnStartHole.AddListener(ViewHole);
        }

        private void OnDisable()
        {
            if (progressHandler) progressHandler.OnStartHole.RemoveListener(ViewHole);
        }

        [ContextMenu("View Hole")]
        private void ViewHole()
        {
            if (ViewTime == 0f) return;

            virtualCamera ??= GetComponent<CinemachineVirtualCamera>();
            viewHoleRoutine ??= StartCoroutine(ViewHoleRoutine());
        }
        private IEnumerator ViewHoleRoutine()
        {
            if (Application.isPlaying) PlayerHandler.SetControls(false);

            var bounds = progressHandler.HoleGenerator.HoleBounds;
            var camZOffset = bounds.size.z + cameraBoundsMargin;
            holeBoundsCenter.position = bounds.center;
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -camZOffset;
            virtualCamera.enabled = true;

            yield return new WaitForSeconds(viewDelay);

            float animationCompletion = 0f, animateSpeed = 1f / ViewTime;
            while (animationCompletion < 1f)
            {
                var angle = Mathf.Lerp(0f, 360f, animationCompletion);
                holeBoundsCenter.rotation = Quaternion.Euler(0f, angle, 0f);

                animationCompletion += animateSpeed * Time.deltaTime;

                yield return null;
            }

            holeBoundsCenter.position = Vector3.zero;
            virtualCamera.enabled = false;

            if (Application.isPlaying) PlayerHandler.SetControls(true);

            viewHoleRoutine = null;
        }
    }
}
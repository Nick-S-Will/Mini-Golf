using Cinemachine;
using MiniGolf.Player;
using MiniGolf.Progress;
using MiniGolf.Terrain;
using System.Collections;
using UnityEngine;

namespace MiniGolf.CameraControl
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class HoleViewer : MonoBehaviour
    {
        [SerializeField] private HoleGenerator holeGenerator;
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

            if (holeGenerator == null) Debug.LogError($"{nameof(holeGenerator)} not assigned");

            ProgressHandler.singleton.OnStartHole.AddListener(ViewHole);
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

            var bounds = holeGenerator.HoleBounds;
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
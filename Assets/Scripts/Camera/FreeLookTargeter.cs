using Cinemachine;
using System.Collections;
using UnityEngine;

namespace MiniGolf.Cinemachine
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    [RequireComponent(typeof(CinemachineInputProvider))]
    public class FreeLookTargeter : CinemachineExtension
    {
        [SerializeField] [Min(0f)] private float cameraLockSeconds = 1f;

        private CinemachineFreeLook freeLookCamera;
        private CinemachineInputProvider inputProvider;
        private Transform target;
        private Coroutine lockRoutine;

        private void Start()
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
            inputProvider = GetComponent<CinemachineInputProvider>();
        }

        public void LookAtNewTarget(MonoBehaviour target)
        {
            this.target = target.transform;
            LookAtTarget();
        }

        public void LookAtTarget()
        {
            var direction = target.position - freeLookCamera.Follow.position;
            var position = freeLookCamera.Follow.position - direction;
            var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            freeLookCamera.ForceCameraPosition(position, lookRotation);

            if (lockRoutine != null) StopCoroutine(lockRoutine);
            if (cameraLockSeconds > 0f) lockRoutine = StartCoroutine(LockCameraRoutine());
        }

        private IEnumerator LockCameraRoutine()
        {
            inputProvider.enabled = false;
            yield return new WaitForSeconds(cameraLockSeconds);
            inputProvider.enabled = true;

            lockRoutine = null;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {}
    }
}
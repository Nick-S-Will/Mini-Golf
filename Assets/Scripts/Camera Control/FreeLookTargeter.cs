using Cinemachine;
using MiniGolf.Player;
using System.Collections;
using UnityEngine;

namespace MiniGolf.CameraControl
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

        protected override void Awake()
        {
            base.Awake();

            freeLookCamera = GetComponent<CinemachineFreeLook>();
            inputProvider = GetComponent<CinemachineInputProvider>();

            PlayerHandler.OnSetPlayer.AddListener(AssignPlayer);
        }

        public void AssignPlayer(SwingController oldPlayer, SwingController newPlayer)
        {
            bool newPlayerExists = newPlayer;
            freeLookCamera.Follow = newPlayerExists ? newPlayer.transform : null;
            freeLookCamera.LookAt = freeLookCamera.Follow;
            freeLookCamera.enabled = newPlayerExists;
        }

        public void LookAtNewTarget(MonoBehaviour target)
        {
            this.target = target ? target.transform : null;
            LookAtTarget();
        }

        public void LookAtTarget()
        {
            if (freeLookCamera.Follow == null)
            {
                Debug.LogError($"{nameof(CinemachineFreeLook)} \"{name}\" has no follow");
                return;
            }
            if (target == null)
            {
                Debug.LogWarning($"{nameof(FreeLookTargeter)} \"{name}\" has no target");
                return;
            }

            var direction = target.position - freeLookCamera.Follow.position;
            var position = freeLookCamera.Follow.position - direction;
            var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            freeLookCamera.ForceCameraPosition(position, lookRotation);

            if (lockRoutine != null) StopCoroutine(lockRoutine);
            if (cameraLockSeconds > 0f) lockRoutine = StartCoroutine(LockCameraRoutine());
        }

        private IEnumerator LockCameraRoutine()
        {
            var startEnabled = inputProvider.enabled;

            inputProvider.enabled = false;
            yield return new WaitForSeconds(cameraLockSeconds);
            inputProvider.enabled = startEnabled;

            lockRoutine = null;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PlayerHandler.OnSetPlayer.RemoveListener(AssignPlayer);
        }
    }
}
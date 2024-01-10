using Cinemachine;
using MiniGolf.Controls;
using UnityEngine;

namespace MiniGolf.Cinemachine
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class DisableCamDuringBackswing : CinemachineExtension
    {
        [SerializeField] private SwingController swingController;

        private CinemachineVirtualCameraBase virtualCamera;

        private void Start()
        {
            if (swingController == null) Debug.LogError($"{nameof(swingController)} not assigned");
            else
            {
                swingController.OnBackswing.AddListener(DisableVCam);
                swingController.OnSwing.AddListener(EnableVCam);
            }

            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();
        }

        private void EnableVCam()
        {
            virtualCamera.enabled = true;
        }

        private void DisableVCam()
        {
            virtualCamera.enabled = false;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
        }
    }
}
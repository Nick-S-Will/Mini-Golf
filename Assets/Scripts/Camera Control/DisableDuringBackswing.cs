using Cinemachine;
using MiniGolf.Player;
using UnityEngine;

namespace MiniGolf.CameraControl
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class DisableDuringBackswing : CinemachineExtension
    {
        private CinemachineVirtualCameraBase virtualCamera;

        protected override void Awake()
        {
            base.Awake();

            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();

            PlayerHandler.OnSetPlayer.AddListener(UpdateTarget);
        }

        private void UpdateTarget(SwingController oldPlayer, SwingController newPlayer)
        {
            if (oldPlayer) RemoveListeners(oldPlayer);
            if (newPlayer == null) return;
            
            virtualCamera.Follow = newPlayer.transform;
            virtualCamera.LookAt = newPlayer.transform;
            AddListeners(newPlayer);
        }

        private void AddListeners(SwingController player)
        {
            player.OnBackswing.AddListener(DisableVirtualCamera);
            player.OnBackswingCancel.AddListener(EnableVirtualCamera);
            player.OnSwing.AddListener(EnableVirtualCamera);
        }

        private void RemoveListeners(SwingController player)
        {
            player.OnBackswing.RemoveListener(DisableVirtualCamera);
            player.OnBackswingCancel.RemoveListener(EnableVirtualCamera);
            player.OnSwing.RemoveListener(EnableVirtualCamera);
        }

        private void EnableVirtualCamera() => virtualCamera.enabled = true;
        private void DisableVirtualCamera() => virtualCamera.enabled = false;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PlayerHandler.OnSetPlayer.RemoveListener(UpdateTarget);
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime){}
    }
}
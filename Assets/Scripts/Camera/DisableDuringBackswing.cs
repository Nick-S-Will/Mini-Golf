using Cinemachine;
using MiniGolf.Player;
using UnityEngine;

namespace MiniGolf.Cinemachine
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class DisableDuringBackswing : CinemachineExtension
    {
        private CinemachineVirtualCameraBase virtualCamera;

        private void Start()
        {
            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();

            PlayerHandler.instance.OnPlayerUpdate.AddListener(UpdateTarget);
            UpdateTarget(null, PlayerHandler.Player);
        }

        private void UpdateTarget(BallController oldPlayer, BallController newPlayer)
        {
            if (oldPlayer) RemoveListeners(oldPlayer);
            if (newPlayer == null) return;
            
            virtualCamera.Follow = newPlayer.transform;
            virtualCamera.LookAt = newPlayer.transform;
            AddListeners(newPlayer);
        }

        private void AddListeners(BallController player)
        {
            player.OnBackswing.AddListener(DisableVirtualCamera);
            player.OnBackswingCancel.AddListener(EnableVirtualCamera);
            player.OnSwing.AddListener(EnableVirtualCamera);
        }

        private void RemoveListeners(BallController player)
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

            if (PlayerHandler.instance) PlayerHandler.instance.OnPlayerUpdate.RemoveListener(UpdateTarget);
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime){}
    }
}
using Cinemachine;
using MiniGolf.Controls;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCameraBase))]
public class DisableCamDuringBackswing : CinemachineExtension
{
    [SerializeField] private BallController ball;

    private CinemachineVirtualCameraBase virtualCamera;

    private void Start()
    {
        if (ball == null) Debug.LogError($"{nameof(ball)} not assigned");
        else
        {
            ball.OnBackswing.AddListener(DisableVCam);
            ball.OnSwing.AddListener(EnableVCam);
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

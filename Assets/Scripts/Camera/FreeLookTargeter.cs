using Cinemachine;
using MiniGolf.Terrain;
using System.Collections;
using UnityEngine;
using static Cinemachine.CinemachineOrbitalTransposer;

namespace MiniGolf.Cinemachine
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class FreeLookTargeter : CinemachineExtension
    {
        private CinemachineFreeLook freeLookCamera;
        private Transform target;

        private void Start()
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
        }

        // For dynamic use on hole generator. Would prefer not needing this dependency
        public void LookAtNewTarget(HoleTile target) => LookAtNewTarget(target.transform);

        public void LookAtNewTarget(Transform target)
        {
            this.target = target;
            LookAtTarget();
        }

        public void LookAtTarget()
        {
            var direction = target.position - freeLookCamera.Follow.position;
            var position = freeLookCamera.Follow.position - direction;
            var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            freeLookCamera.ForceCameraPosition(position, lookRotation);
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {}
    }
}
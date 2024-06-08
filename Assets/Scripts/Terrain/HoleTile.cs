using MiniGolf.Player;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    public class HoleTile : Tile
    {
        [Space]
        public UnityEvent<SwingController> OnBallEnter;

        public int BallCount { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            var ballController = other.GetComponent<SwingController>();
            if (ballController)
            {
                BallCount++;
                OnBallEnter.Invoke(ballController);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var ballController = other.GetComponent<SwingController>();
            if (ballController) BallCount--;
        }
    }
}
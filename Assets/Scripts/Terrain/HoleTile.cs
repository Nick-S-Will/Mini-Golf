using MiniGolf.Player;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    public class HoleTile : Tile
    {
        [Space]
        public UnityEvent<BallController> OnBallEnter;

        private void OnTriggerEnter(Collider other)
        {
            var ballController = other.GetComponent<BallController>();
            if (ballController) OnBallEnter.Invoke(ballController);
        }
    }
}
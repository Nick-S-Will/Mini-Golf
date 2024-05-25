using MiniGolf.Player;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    public class HoleTile : Tile
    {
        [Space]
        public UnityEvent<SwingController> OnBallEnter;

        private void OnTriggerEnter(Collider other)
        {
            var ballController = other.GetComponent<SwingController>();
            if (ballController) OnBallEnter.Invoke(ballController);
        }
    }
}
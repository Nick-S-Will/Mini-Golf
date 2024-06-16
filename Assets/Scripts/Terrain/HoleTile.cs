using MiniGolf.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    public class HoleTile : Tile
    {
        [Space]
        public UnityEvent<SwingController> OnBallEnter;

        private readonly HashSet<SwingController> heldBalls = new();

        public int BallCount => heldBalls.Count;

        public bool Contains(SwingController controller) => heldBalls.Contains(controller);

        private void OnTriggerEnter(Collider other)
        {
            var swingController = other.GetComponent<SwingController>();
            if (swingController)
            {
                heldBalls.Add(swingController);
                OnBallEnter.Invoke(swingController);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var swingController = other.GetComponent<SwingController>();
            if (swingController) _ = heldBalls.Remove(swingController);
        }
    }
}
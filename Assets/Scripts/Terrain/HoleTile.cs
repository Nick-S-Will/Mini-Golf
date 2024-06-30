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

        public int BallCount
        {
            get
            {
                heldBalls.RemoveWhere(player => player == null);
                return heldBalls.Count;
            }
        }

        public bool Contains(SwingController controller) => heldBalls.Contains(controller);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out SwingController player)) return;

            _ = heldBalls.Add(player);
            OnBallEnter.Invoke(player);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out SwingController player)) return;

            _ = heldBalls.Remove(player);
        }
    }
}
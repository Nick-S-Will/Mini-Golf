using MiniGolf.Controls;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    public class HoleTile : CourseTile
    {
        public UnityEvent OnBallEnter;

        private void OnTriggerEnter(Collider other)
        {
            var ballController = other.GetComponent<BallController>();
            if (ballController) OnBallEnter.Invoke();
        }
    }
}
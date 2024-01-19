using MiniGolf.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    public class HoleTile : CourseTile
    {
        public UnityEvent<BallController> OnBallEnter;

        private void OnTriggerEnter(Collider other)
        {
            var ballController = other.GetComponent<BallController>();
            if (ballController) OnBallEnter.Invoke(ballController);
        }
    }
}
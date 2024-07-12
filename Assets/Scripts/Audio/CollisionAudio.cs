using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Audio
{
    [RequireComponent(typeof(Collider))]
    public class CollisionAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource collisionEnterAudioSource, collisionStayAudioSource;

        private HashSet<Rigidbody> contacts = new();

        private void Awake()
        {
            if (collisionEnterAudioSource == null) Debug.LogError($"{nameof(collisionEnterAudioSource)} not assigned");
            if (collisionStayAudioSource == null) Debug.LogError($"{nameof(collisionStayAudioSource)} not assigned");
        }

        private void Update()
        {
            contacts.RemoveWhere(rigidbody => rigidbody == null);
            if (contacts.Count == 0) return;

            var position = contacts.Average(rigidbody => rigidbody.position);
            collisionEnterAudioSource.transform.position = position;
            collisionStayAudioSource.transform.position = position;

            var volume = Mathf.Clamp01(contacts.Average(rigidbody => rigidbody.velocity.magnitude));
            collisionEnterAudioSource.volume = volume;
            collisionStayAudioSource.volume = volume;
        }

        public void StartCollision(Rigidbody rigidbody)
        {
            collisionEnterAudioSource.Play();
            if (contacts.Count == 0) collisionStayAudioSource.Play();

            contacts.Add(rigidbody);
        }

        public void StopCollision(Rigidbody rigidbody)
        {
            contacts.Remove(rigidbody);

            if (contacts.Count == 0) collisionStayAudioSource.Stop();
        }
    }

    internal static class VectorExtensions
    {
        public static Vector3 Average<T>(this IEnumerable<T> source, Func<T, Vector3> selector)
        {
            var sum = Vector3.zero;
            foreach (var obj in source) sum += selector(obj);
            return 1f / source.Count() * sum;
        }
    }
}
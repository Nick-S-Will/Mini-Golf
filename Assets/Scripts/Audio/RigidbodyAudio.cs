using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Audio
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource collisionEnterAudioSource, collisionStayAudioSource;

        private HashSet<Collider> contacts = new();
        private new Rigidbody rigidbody;

        private void Awake()
        {
            if (collisionEnterAudioSource == null) Debug.LogError($"{nameof(collisionEnterAudioSource)} not assigned");
            if (collisionStayAudioSource == null) Debug.LogError($"{nameof(collisionStayAudioSource)} not assigned");

            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            contacts.RemoveWhere(collider => collider == null);
            if (contacts.Count == 0) return;

            var volume = Mathf.Clamp01(rigidbody.velocity.magnitude);
            collisionEnterAudioSource.volume = volume;
            collisionStayAudioSource.volume = volume;
        }

        private void OnCollisionEnter(Collision collision)
        {
            StartCollision(collision.collider);

            if (collision.gameObject.TryGetComponent(out CollisionAudio collisionAudio))
            {
                collisionAudio.StartCollision(rigidbody);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            StopCollision(collision.collider);

            if (collision.gameObject.TryGetComponent(out CollisionAudio collisionAudio))
            {
                collisionAudio.StopCollision(rigidbody);
            }
        }

        public void StartCollision(Collider collider)
        {
            collisionEnterAudioSource.Play();
            if (contacts.Count == 0) collisionStayAudioSource.Play();

            contacts.Add(collider);
        }

        public void StopCollision(Collider collider)
        {
            contacts.Remove(collider);

            if (contacts.Count == 0) collisionStayAudioSource.Stop();
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem collisionEnterParticlePrefab;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        if (collisionEnterParticlePrefab == null) Debug.LogError($"{nameof(collisionEnterParticlePrefab)} not assigned");

        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0);
        var collisionEnterParticles = Instantiate(collisionEnterParticlePrefab, contact.point, Quaternion.identity);
        collisionEnterParticles.transform.up = contact.normal;

        var collisionEnterMain = collisionEnterParticles.main;
        collisionEnterMain.startSpeed = rigidbody.velocity.magnitude;
        collisionEnterParticles.Play();
    }
}
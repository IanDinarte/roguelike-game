using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    private int damage;

    public float maxDistance = 50f;
    private Vector3 initialPosition;
    private BoxCollider boxCollider;
    private Rigidbody rigidBody;
    [SerializeField] private GameObject dustGameObject;
    private bool isStuck = false;
    private float maxLifeTime = 60f;

    [SerializeField] private AudioSource impactSound;

    public void SetDamage(int newDamage) => damage = newDamage;

    private void Start()
    {
        initialPosition = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();

        LifeTime(maxLifeTime);
    }

    private void Update()
    {
        float distanceTraveled = Vector3.Distance(initialPosition, transform.position);

        if (distanceTraveled > maxDistance && !isStuck)
        {
            Destroy(gameObject);
        }
    }

    private async void LifeTime(float lifeTime)
    {
        await Awaitable.WaitForSecondsAsync(lifeTime);
        if (this != null) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Arrow Hit " + other.name);
            HitController player = other.GetComponent<HitController>();
            //Destroy(gameObject);
            if (player != null)
            {
                player.TakeDamage(damage, damage, false, false);
                isStuck = true;
                gameObject.transform.SetParent(other.transform.GetChild(0));

                gameObject.layer = LayerMask.NameToLayer("Player");
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Player");
                }

                boxCollider.enabled = false;
                rigidBody.isKinematic = true;
            }
        }
        else if (other.CompareTag("Shield"))
        {
            //Debug.Log("Arrow Hit " + other.name);
            HitController player = other.GetComponentInParent<HitController>();
            Destroy(gameObject);
            if (player != null)
            {
                player.TakeDamage(damage, damage, true, false);
            }
        }
        else if (other.CompareTag("Terrain"))
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            dustGameObject.transform.position = collisionPoint;

            ParticleSystem dustParticle = dustGameObject.GetComponent<ParticleSystem>();
            if (dustParticle != null)
            {
                dustParticle.Play();
            }

            impactSound.Play();
            //Debug.Log("Arrow Hit" + other.name);
            //Destroy(gameObject);
            isStuck = true;
            boxCollider.enabled = false;
            rigidBody.isKinematic = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int[] damage;

    public float maxDistance = 20f;
    private Vector3 initialPosition;
    private BoxCollider boxCollider;
    private Rigidbody rigidBody;
    [SerializeField] private GameObject dustGameObject;
    private bool isStuck = false;

    [SerializeField] private Renderer headMaterial;
    [SerializeField] private Renderer bodyMaterial;
    private Color critEffectColor = new Color32(60, 0, 191, 255);
    private float critEffectFadeDuration = 1f;

    [SerializeField] private AudioSource impactSound;

    public void SetDamage(int[] damage)
    {
        this.damage = damage;
    }

    private void Start()
    {
        initialPosition = transform.position;
        boxCollider = this.GetComponent<BoxCollider>();
        rigidBody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float distanceTraveled = Vector3.Distance(initialPosition, transform.position);

        if (distanceTraveled > maxDistance && !isStuck)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("Arrow Hit " + other.name);
            EnemyHitController enemy = other.GetComponent<EnemyHitController>();
            //Destroy(gameObject);
            if (enemy != null)
            {
                enemy.TakeDamage(damage[0], damage[1]);
                isStuck = true;
                gameObject.transform.SetParent(other.transform.GetChild(1));
                boxCollider.enabled = false;
                rigidBody.isKinematic = true;
            }
        }
        else if (other.CompareTag("Boss"))
        {
            BossHitController boss = other.GetComponent<BossHitController>();
            if (boss != null)
            {
                boss.TakeDamage(damage[0], damage[1]);
                isStuck = true;
                gameObject.transform.SetParent(other.transform.GetChild(1));
                boxCollider.enabled = false;
                rigidBody.isKinematic = true;
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
        else if (other.CompareTag("Destructible"))
        {
            impactSound.Play();
            //play sound or whatever
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public async void PlayCritEffect()
    {
        float intensity = 10f;

        bodyMaterial.material.EnableKeyword("_EMISSION");
        bodyMaterial.material.SetColor("_EmissionColor", critEffectColor * intensity);

        headMaterial.material.EnableKeyword("_EMISSION");
        headMaterial.material.SetColor("_EmissionColor", critEffectColor * intensity);

        await Awaitable.WaitForSecondsAsync(critEffectFadeDuration * 0.5f);

        float elapsedTime = 0f;

        while (elapsedTime < critEffectFadeDuration * 0.5f)
        {
            elapsedTime += Time.deltaTime;
            intensity -= 10 / (critEffectFadeDuration * 0.5f) * Time.deltaTime;

            bodyMaterial.material.SetColor("_EmissionColor", critEffectColor * intensity);
            headMaterial.material.SetColor("_EmissionColor", critEffectColor * intensity);

            await Awaitable.NextFrameAsync();
        }

        bodyMaterial.material.SetColor("_EmissionColor", critEffectColor * -10);
        headMaterial.material.SetColor("_EmissionColor", critEffectColor * -10);

        bodyMaterial.material.DisableKeyword("_EMISSION");
        headMaterial.material.DisableKeyword("_EMISSION");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DarkSpawnHitController : EnemyHitController
{
    private EnemyAttackController attackController;
    private EnemyController enemyController;

    private float dissolveDuration = 10f;

    public DarkSpawnHitController()
    {
        this.SetMaxHealth(30);
        this.SetHealth(30);
        this.SetMaxPoise(0);
        this.SetPoise(0);
    }

    protected override void Awake()
    {
        animator = this.GetComponent<Animator>();
        capsuleCollider = this.GetComponent<CapsuleCollider>();
        attackController = this.GetComponent<EnemyAttackController>();
        enemyController = this.GetComponent<EnemyController>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        rigidBody = this.GetComponent<Rigidbody>();
        lootController = this.GetComponent<EnemyLootController>();
    }

    private void Start()
    {
        StartCoroutine(Materialize());
        playerAttack = enemyController.player.gameObject.GetComponent<AttackController>();
        isInvincible = true;
    }

    private void Update()
    {
        if (health <= 0 && !IsDead)
        {
            Die();
        }
    }

    private IEnumerator Materialize()
    {
        float elapsedTime = 0f;
        float duration = 3f;

        Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y - 1.5f, this.transform.position.z);
        this.transform.position = pos;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            pos.y += 0.5f * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        isInvincible = false;
    }

    public override void TakeDamage(int healthDamage, int poiseDamage)
    {
        if (!IsDead && !isInvincible)
        {
            health -= healthDamage;
            HitSound.Play();
            bloodSplatter.Play();
        }
    }

    public override void KnockBack(int healthDamage, int poiseDamage, float knockbackForce, float knockbackDuration, Transform source)
    {
        if (!IsDead && !isInvincible)
        {
            health -= healthDamage;
            HitSound.Play();
            bloodSplatter.Play();
            // if (health <= 0)
            // {
            //     Die();
            // }
            if (!attackController.isAttacking && !knocked)
            {
                animator.SetInteger("Knocked", 1);

                this.navMeshAgent.enabled = false;

                Vector3 knockbackDirection = (this.transform.position - source.position).normalized;
                knockbackDirection.y = 0;
                knockbackDirection.Normalize();

                this.rigidBody.isKinematic = false;
                this.rigidBody.linearVelocity = Vector3.zero; // Reset the velocity
                this.rigidBody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

                StartCoroutine(WaitForKnockback(knockbackDuration));
            }
        }
    }

    private IEnumerator WaitForKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetInteger("Knocked", 2);
    }

    public void OnKnockedAnimation()
    {
        knocked = true;
        this.navMeshAgent.enabled = false;
        animator.SetInteger("Knocked", 3);
    }

    public void OnGetUpAnimation()
    {
        animator.SetInteger("Knocked", 3);
        this.rigidBody.isKinematic = true;
    }

    public void OnGetUpAnimationEnd()
    {
        knocked = false;
        this.navMeshAgent.enabled = true;
        animator.SetInteger("Knocked", 0);
        Debug.Log("NavMeshAgent re-enabled");
    }

    public override void OnHitAnimationEnd()
    {
    }

    protected override void Die()
    {
        playerAttack.onEnemySlain();

        rigidBody.isKinematic = true;

        animator.SetInteger("Death", 1);
        IsDead = true;
        capsuleCollider.enabled = false;
        navMeshAgent.enabled = false;
        blackSludge.Stop();
        lootController.DropLoot();
    }

    protected override void OnDeathAnimation()
    {
        animator.SetInteger("Death", 0);
    }

    private IEnumerator Dissolve(float duration)
    {
        float elapsedTime = 0f;
        Vector3 pos = this.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            pos.y -= 0.5f * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        Destroy(gameObject);
    }

    protected void OnDeathAnimationEnd()
    {
        StartCoroutine(Dissolve(dissolveDuration));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class WraithHitController : EnemyHitController
{
    private EnemyAttackController attackController;
    private EnemyController enemyController;

    public WraithHitController()
    {
        SetMaxHealth(100);
        SetHealth(100);
        SetMaxPoise(50);
        SetPoise(50);
    }

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        //boxCollider = this.GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        attackController = GetComponent<EnemyAttackController>();
        enemyController = GetComponent<EnemyController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        lootController = GetComponent<EnemyLootController>();
    }

    private void Start()
    {
        playerAttack = enemyController.player.gameObject.GetComponent<AttackController>();
    }

    public override void TakeDamage(int healthDamage, int poiseDamage)
    {
        if (!IsDead)
        {
            health -= healthDamage;
            poise -= poiseDamage;
            HitSound.Play();
            bloodSplatter.Play();
            enemyController.DamageAggro();
            //Debug.Log("Damage received: " + healthDamage);
            if (health <= 0)
            {
                Die();
            }
            else if (poise <= 0 && !knocked)
            {
                HitGrunt.Play();
                poise = maxPoise;
                animator.SetInteger("Hit", 1);
            }
        }
    }

    public override void KnockBack(int healthDamage, int poiseDamage, float knockbackForce, float knockbackDuration, Transform source)
    {
        if (!IsDead)
        {
            health -= healthDamage;
            poise -= poiseDamage;
            HitSound.Play();
            bloodSplatter.Play();
            enemyController.DamageAggro();
            if (health <= 0)
            {
                Die();
            }
            else if (!knocked)
            {
                animator.SetInteger("Knocked", 1);

                navMeshAgent.enabled = false;

                // Calculate the knockback direction
                Vector3 knockbackDirection = (transform.position - source.position).normalized;
                knockbackDirection.y = 0;
                knockbackDirection.Normalize();

                rigidBody.isKinematic = false;
                rigidBody.linearVelocity = Vector3.zero; // Reset the velocity
                rigidBody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

                StartCoroutine(WaitForKnockback(knockbackDuration));
            }
        }
    }

    private async void StaggerAsync(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }

        isStaggered = false;
        navMeshAgent.isStopped = false;
    }

    private IEnumerator WaitForKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);
        /* agent.enabled = true;
        rigidbody.velocity = Vector3.zero;
        Debug.Log("NavMeshAgent re-enabled"); */
        animator.SetInteger("Knocked", 2);
    }

    public void OnKnockedAnimation()
    {
        knocked = true;
        //navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        //navMeshAgent.updateRotation = false;
        animator.SetInteger("Knocked", 3);
    }

    public void OnGetUpAnimation()
    {
        animator.SetInteger("Knocked", 3);
        rigidBody.isKinematic = true;
    }

    public void OnGetUpAnimationEnd()
    {
        knocked = false;
        //navMeshAgent.isStopped = false;
        //navMeshAgent.updateRotation = true;
        navMeshAgent.enabled = true;
        //this.rigidbody.velocity = Vector3.zero;
        animator.SetInteger("Knocked", 0);
        //Debug.Log("NavMeshAgent re-enabled");
    }

    public void OnHitAnimation()
    {
        animator.SetInteger("Hit", 0);

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Hit"))
        {
            navMeshAgent.isStopped = true;
            isStaggered = true;
            StaggerAsync(stateInfo.length);
        }
    }

    public override void OnHitAnimationEnd()
    {
        // navMeshAgent.isStopped = false;
        // Debug.Log("movement reenabled");
    }

    protected override void Die()
    {
        playerAttack.onEnemySlain();

        rigidBody.isKinematic = true;

        IsDead = true;
        int deathAnim = Random.Range(1, 4);
        animator.SetInteger("Death", deathAnim);
        capsuleCollider.enabled = false;
        navMeshAgent.enabled = false;
        ClearHands();
        blackSludge.Stop();
        redEyes.SetActive(false);
        lootController.DropLoot();
        //boxCollider.enabled = false;
        //Destroy(gameObject);

        DeathSound.Play();
    }


    protected override void OnDeathAnimation()
    {
        //play death sound?
        animator.SetInteger("Death", 0);
    }
}

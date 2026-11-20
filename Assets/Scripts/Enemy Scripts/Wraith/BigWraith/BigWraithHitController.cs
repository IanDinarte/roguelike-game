using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigWraithHitController : EnemyHitController
{
    private EnemyAttackController attackController;
    private EnemyController enemyController;

    public BigWraithHitController()
    {
        this.SetMaxHealth(300);
        this.SetHealth(300);
        this.SetMaxPoise(150);
        this.SetPoise(150);
    }

    protected override void Awake()
    {
        animator = this.GetComponent<Animator>();
        //boxCollider = this.GetComponent<BoxCollider>();
        capsuleCollider = this.GetComponent<CapsuleCollider>();
        attackController = this.GetComponent<EnemyAttackController>();
        enemyController = this.GetComponent<EnemyController>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        rigidBody = this.GetComponent<Rigidbody>();
        lootController = this.GetComponent<EnemyLootController>();
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
                poise = maxPoise;
                navMeshAgent.isStopped = true;
                animator.SetTrigger("Hit");
            }
        }
    }

    public override void KnockBack(int healthDamage, int poiseDamage, float knockbackForce, float knockbackDuration, Transform source)
    {
        if (!IsDead)
        {
            health -= healthDamage;
            poise -= poiseDamage / 2;
            HitSound.Play();
            bloodSplatter.Play();
            enemyController.DamageAggro();
            if (health <= 0)
            {
                Die();
            }
            else if (poise <= 0 && !knocked)
            {
                poise = maxPoise;
                animator.SetInteger("Hit", 1);
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
        //navMeshAgent.isStopped = false;
    }

    protected override void Die()
    {
        playerAttack.onEnemySlain();

        IsDead = true;
        int deathAnim = Random.Range(1, 3);
        animator.SetInteger("Death", deathAnim);
        capsuleCollider.enabled = false;
        navMeshAgent.enabled = false;
        ClearHands();
        lootController.DropLoot();
        //boxCollider.enabled = false;
        //Destroy(gameObject);
    }

    protected override void OnDeathAnimation()
    {
        //play death sound?
        animator.SetInteger("Death", 0);
    }
}

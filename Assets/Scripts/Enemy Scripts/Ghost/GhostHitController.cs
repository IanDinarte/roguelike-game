using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostHitController : EnemyHitController
{
    private GhostAttackController attackController;
    private EnemyController enemyController;

    public GhostHitController()
    {
        SetMaxHealth(60);
        SetHealth(60);
        SetMaxPoise(40);
        SetPoise(40);
    }

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        //boxCollider = this.GetComponent<BoxCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        attackController = GetComponent<GhostAttackController>();
        rigidBody = GetComponent<Rigidbody>();
        enemyController = GetComponent<EnemyController>();
        lootController = GetComponent<EnemyLootController>();
    }

    private void Start()
    {
        playerAttack = enemyController.player.gameObject.GetComponent<AttackController>();
    }

    private void Update()
    {
        if (health <= 0 && !IsDead)
        {
            Die();
        }
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
            if (!attackController.isAttacking && !attackController.isCharging
            && !attackController.isGrabbing && !attackController.isCasting && poise <= 0 && !knocked)
            {
                HitGrunt.Play();
                poise = maxPoise;
                isStaggered = true;
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
            poise -= poiseDamage;
            HitSound.Play();
            bloodSplatter.Play();
            enemyController.DamageAggro();
            if (!attackController.isAttacking && !attackController.isCharging
            && !attackController.isGrabbing && !attackController.isCasting && poise <= 0 && !knocked)
            {
                HitGrunt.Play();
                poise = maxPoise;
                navMeshAgent.isStopped = true;
                animator.SetTrigger("Hit");
            }
        }
    }

    public override void OnHitAnimationEnd()
    {
        isStaggered = false;
        navMeshAgent.isStopped = false;
    }

    protected override void Die()
    {
        playerAttack.onEnemySlain();
        lootController.DropLoot();

        navMeshAgent.isStopped = true;
        capsuleCollider.enabled = false;
        navMeshAgent.enabled = false;
        
        animator.SetInteger("Death", 1);

        DeathSound.Play();
        //boxCollider.enabled = false;
        IsDead = true;
    }

    protected override void OnDeathAnimation()
    {
        //play death sound?
        animator.SetInteger("Death", 2);
    }
}

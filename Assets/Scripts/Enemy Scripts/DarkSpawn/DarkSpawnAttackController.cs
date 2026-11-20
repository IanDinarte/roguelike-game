using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DarkSpawnAttackController : EnemyAttackController
{
    private EnemyWeaponController weaponController;

    DarkSpawnAttackController()
    {
        this.SetAttackRange(2f);
        this.SetAttackCooldown(1.5f);
        this.SetStandTime(0.1f);
    }

    protected override void Awake()
    {
        animator = this.GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        hitController = this.GetComponent<EnemyHitController>();
        weaponController = this.GetComponentInChildren<EnemyWeaponController>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public override void PrepareAttack()
    {
        RotateTowardsPlayer();
        if (Time.time >= lastAttackTime + attackCooldown
            && IsFacingPlayer()
            && !hitController.knocked
            && !hitController.isStaggered)
        {
            AttackPlayer();
        }
    }

    public override void OnAttackAnimation()
    {
        navMeshAgent.isStopped = true;

        animator.SetInteger("Atk", 2);

        lastAttackTime = Time.time;
    }

    public override void AttackSound()
    {
        attackSound.Play();
        canRotate = false;

        isAttacking = true;
        weaponController.StartAttack();
    }

    public override void OnAttackAnimationEnd()
    {
        isAttacking = false;

        canRotate = true;
        animator.SetInteger("Atk", 0);
        weaponController.EndAttack();

        StartCoroutine(StandTime(standTime));
    }
}
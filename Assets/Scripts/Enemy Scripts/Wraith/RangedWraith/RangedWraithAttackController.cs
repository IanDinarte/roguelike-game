using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedWraithAttackController : EnemyAttackController
{
    private EnemyRangedWeaponController rangedWeaponController;

    public RangedWraithAttackController()
    {
        this.SetAttackRange(10f);
        this.SetAttackCooldown(2f);
        this.SetStandTime(1f);
    }

    protected override void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        hitController = this.GetComponent<EnemyHitController>();
        rangedWeaponController = this.GetComponentInChildren<EnemyRangedWeaponController>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // void Update()
    // {
    //     /* if (!hitController.isDead && !isAttacking && navMeshAgent.velocity.magnitude < 0.1f)
    //     {
    //         RotateTowardsPlayer();
    //     } */
    // }

    public override void PrepareAttack()
    {
        if (navMeshAgent.velocity == Vector3.zero)
        {
            RotateTowardsPlayer();
            if (Time.time >= lastAttackTime + attackCooldown && IsFacingPlayer() && !hitController.knocked && !hitController.isStaggered)
            {
                AttackPlayer();
            }
        }
    }

    //stand still time after attacking
    // private IEnumerator StandTime(float duration)
    // {
    //     yield return new WaitForSeconds(duration);
    //     navMeshAgent.isStopped = false;
    // }

    // bool IsFacingPlayer()
    // {
    //     Vector3 directionToPlayer = (player.position - transform.position).normalized;
    //     float angle = Vector3.Angle(transform.forward, new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
    //     return angle < rotationThreshold;
    // }

    // void RotateTowardsPlayer()
    // {
    //     Vector3 direction = (player.position - transform.position).normalized;
    //     Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    //     transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    // }

    // void AttackPlayer()
    // {
    //     animator.SetInteger("Atk", 1);
    // }

    public override void AttackSound()
    {
        attackSound.Play();

        rangedWeaponController.ShootArrow();
    }

    private async void AttackAsync(float duration)
    {
        await Awaitable.WaitForSecondsAsync(duration);

        isAttacking = false;

        StartCoroutine(StandTime(standTime));
    }

    public override void OnAttackAnimation()
    {
        navMeshAgent.isStopped = true;
        RotateTowardsPlayer();

        isAttacking = true;

        lastAttackTime = Time.time;

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Shoot"))
        {
            AttackAsync(stateInfo.length);
        }

        animator.SetInteger("Atk", 0);
    }

    //called through animation event
    public override void OnAttackAnimationEnd()
    {
        // isAttacking = false;
        // //navMeshAgent.isStopped = false; // Resume the enemy's movement

        // //rangedWeaponController.EndAttack();

        // StartCoroutine(StandTime(standTime));
    }
}

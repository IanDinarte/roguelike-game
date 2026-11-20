using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAttackController : MonoBehaviour
{
    public Transform player;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    public AudioSource attackSound;
    protected EnemyHitController hitController;

    //rotation variables
    public float rotationSpeed = 30.0f;
    public float rotationThreshold = 5.0f;

    //attack variables //damage is in the weapon
    public float attackRange;
    public float attackCooldown;
    public bool isAttacking = false;
    protected float lastAttackTime;
    public float standTime;
    public bool canRotate = true;

    public void SetAttackRange(float attackRange)
    {
        this.attackRange = attackRange;
    }

    public void SetAttackCooldown(float attackCooldown)
    {
        this.attackCooldown = attackCooldown;
    }

    public void SetStandTime(float standTime)
    {
        this.standTime = standTime;
    }

    protected abstract void Awake();

    public abstract void PrepareAttack();

    protected IEnumerator StandTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        navMeshAgent.isStopped = false;
    }

    public bool IsFacingPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        return angle < rotationThreshold;
    }

    protected void RotateTowardsPlayer()
    {
        if (canRotate)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    protected void AttackPlayer()
    {
        animator.SetInteger("Atk", 1);
    }

    public abstract void AttackSound();

    public abstract void OnAttackAnimation();

    public abstract void OnAttackAnimationEnd();
}

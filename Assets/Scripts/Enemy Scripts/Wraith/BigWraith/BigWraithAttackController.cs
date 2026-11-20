using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigWraithAttackController : EnemyAttackController
{
    private EnemyWeaponController weaponController;
    private BigWraithController enemyController;

    public ParticleSystem tauntEffect;

    public bool isTaunting = false;
    public bool canTaunt = true;
    public float tauntMinRange = 10f; //not taunt range exactly, but the range he will taunt
    public float tauntMaxRange = 15f;
    private float tauntCooldown = 15f;
    private float slowDuration = 5f;
    private float slowStrength = 1.3f;

    public BigWraithAttackController()
    {
        this.SetAttackRange(3f);
        this.SetAttackCooldown(3f);
        this.SetStandTime(0f);
    }

    protected override void Awake()
    {
        animator = this.GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        hitController = this.GetComponent<EnemyHitController>();
        weaponController = this.GetComponentInChildren<EnemyWeaponController>();
        enemyController = this.GetComponent<BigWraithController>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public override void PrepareAttack()
    {
        RotateTowardsPlayer();
        if (!isAttacking && !isTaunting && !hitController.isStaggered)
        {
            if (Time.time >= lastAttackTime + attackCooldown && IsFacingPlayer())
            {
                //navMeshAgent.ResetPath();
                AttackPlayer();
            }
        }
    }

    private IEnumerator TauntCooldown(float tauntCooldown)
    {
        yield return new WaitForSeconds(tauntCooldown);
        canTaunt = true;
    }

    public void PrepareTaunt()
    {
        if (!isAttacking && !isTaunting && !hitController.isStaggered)
        {
            animator.SetInteger("Taunt", 1);
        }
    }

    private async void AttackAsync(float duration)
    {
        await Awaitable.WaitForSecondsAsync(duration);

        isAttacking = false;
        weaponController.EndAttack();

        canRotate = true;
        navMeshAgent.isStopped = false;
    }

    public override void OnAttackAnimation()
    {
        isAttacking = true;
        //RotateTowardsPlayer();
        navMeshAgent.isStopped = true;
        lastAttackTime = Time.time;

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Enemy Attack Start"))
        {
            AttackAsync(stateInfo.length);
        }

        animator.SetInteger("Atk", 0);
    }

    public override void AttackSound()
    {
        //canRotate = false;
        attackSound.Play();
        weaponController.StartAttack();
    }

    public override void OnAttackAnimationEnd()
    {
        // isAttacking = false;
        // weaponController.EndAttack();

        // canRotate = true;
        // navMeshAgent.isStopped = false;

        //StartCoroutine(StandTime(standTime));
    }

    private async void TauntAsync(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }

        canTaunt = false;
        isTaunting = false;
        navMeshAgent.isStopped = false;
        StartCoroutine(TauntCooldown(tauntCooldown));
    }

    public void OnTauntAnimation()
    {
        animator.SetInteger("Taunt", 0);

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Taunt"))
        {
            if (enemyController.distanceToPlayer < tauntMaxRange)
            {
                HitController player = enemyController.player.GetComponent<HitController>();
                player.Slow(slowDuration, slowStrength);
            }

            tauntEffect.Play();
            isTaunting = true;
            navMeshAgent.isStopped = true;
            TauntAsync(stateInfo.length);
        }
    }

    public void OnTauntAnimationEnd()
    {
        // canTaunt = false;
        // isTaunting = false;
        // navMeshAgent.isStopped = false;
        // StartCoroutine(TauntCooldown(tauntCooldown));
    }
}

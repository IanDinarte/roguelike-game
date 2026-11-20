using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

//cant grab whos already stunned, but make so when player is stunned he has no colision otherwise other ghosts will be pushing him around
public class GhostAttackController : EnemyAttackController
{
    private GhostWeaponController weaponController;
    private GhostController ghostController;
    private GrabController grabController;
    private SkinnedMeshRenderer meshRenderer;

    [SerializeField] private AudioSource chargeSound;

    //materials
    public Material invisibleMat; //absolute videogame
    public Material opaqueMat;

    //grab variables
    public bool isGrabbing = false; //divide isAttacking into the two stages of the charge attack / Also used when missed the charge to prevent from regular attacking right after
    public float grabDuration = 2.1f;
    private int originalLayer;
    private CancellationTokenSource cts;

    //charge variables, charge ranges used in ghost controller
    public bool canCharge = true;
    public bool isCharging = false;
    public float chargeRange = 10.0f;
    public float chargeMinRange = 3.0f;
    private float chargeSpeed = 15.0f;
    private float chargeAcceleration = 10.0f;
    private float chargeDuration = 2f;
    private float chargeCooldown = 10.0f;
    private float regularSpeed = 3.5f;
    private float regularAcceleration = 999.0f;

    //flee mode
    public bool isCasting = false;
    private float invisibleDuration = 3.0f;

    public GhostAttackController()
    {
        SetAttackRange(1.2f);
        SetAttackCooldown(2f);
        SetStandTime(1f);
    }

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        hitController = GetComponent<EnemyHitController>();
        weaponController = GetComponentInChildren<GhostWeaponController>();
        ghostController = GetComponent<GhostController>();
        grabController = GetComponentInChildren<GrabController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        originalLayer = gameObject.layer;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // if (isCharging)
        // {
        //     navMeshAgent.SetDestination(player.position);
        // }
    }

    public void PrepareChargeAttack()
    {
        //RotateTowardsPlayer();
        if (!isCharging && !hitController.isStaggered && navMeshAgent.isStopped == false)
        {
            //Debug.Log("charging");
            BeginChargeAttack();
        }
    }

    public override void PrepareAttack()
    {
        if (!isCharging && !isGrabbing && !isCasting)
        {
            RotateTowardsPlayer();
            if (Time.time >= lastAttackTime + attackCooldown && IsFacingPlayer())
            {
                AttackPlayer();
            }
        }
    }

    private async void ChargeCooldown(float chargeCooldown)
    {
        await Awaitable.WaitForSecondsAsync(chargeCooldown);

        isCharging = false;
        canCharge = true;
    }

    private async void ChargeTime(float chargeDuration, CancellationToken token)
    {
        float elapsedTime = 0f;

        while (!token.IsCancellationRequested && elapsedTime < chargeDuration && !hitController.IsDead)
        {
            elapsedTime += Time.deltaTime;

            navMeshAgent.SetDestination(player.position);
            
            await Awaitable.NextFrameAsync();
        }

        grabController.EndAttack();

        if (!isGrabbing && !hitController.IsDead)
        {
            //isCharging = false;
            GrabFailed();
        }
        else if (hitController.IsDead)
        {
            animator.SetInteger("Charge", 0);
        }
    }

    private IEnumerator GrabTime(float grabDuration)
    {
        yield return new WaitForSeconds(grabDuration);

        navMeshAgent.isStopped = false;
        //isGrabbing = false;
        isCasting = true;
        animator.SetInteger("Charge", 4);
    }

    private async void InvisibleTime(float fleeDuration)
    {
        await Awaitable.WaitForSecondsAsync(fleeDuration);

        ghostController.isInvisible = false;
        //renderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        meshRenderer.material = opaqueMat;
        gameObject.layer = originalLayer;
    }

    public void Invisibility()
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
        isGrabbing = false;
        isCasting = false;
        ghostController.isInvisible = true;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = false;
        InvisibleTime(invisibleDuration);
        ChargeCooldown(chargeCooldown);
        //renderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        meshRenderer.material = invisibleMat;
    }

    void BeginChargeAttack() //begin charge attack
    {
        cts = new CancellationTokenSource();

        canCharge = false;
        isCharging = true;
        navMeshAgent.speed = chargeSpeed;
        navMeshAgent.acceleration = chargeAcceleration;
        animator.SetInteger("Charge", 1);
        grabController.StartAttack();
        ChargeTime(chargeDuration, cts.Token);

        chargeSound.Play();
    }

    public void GrabFailed()
    {
        hitController.HitGrunt.Play();

        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        //grabController.EndAttack();
        navMeshAgent.speed = regularSpeed;
        navMeshAgent.acceleration = regularAcceleration;

        animator.SetInteger("Charge", 3);
        navMeshAgent.isStopped = true;
        isCharging = false;
        isCasting = true;
    }

    public void GrabSuccessful()
    {
        //StopCoroutine(ChargeTime());
        navMeshAgent.speed = regularSpeed;
        navMeshAgent.acceleration = regularAcceleration;
        isCharging = false;
        isGrabbing = true;
        navMeshAgent.isStopped = true;
        animator.SetInteger("Charge", 2);
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
        StartCoroutine(GrabTime(grabDuration));
    }

    public override void AttackSound()
    {
        attackSound.Play();
        weaponController.StartAttack();
    }

    private async void AttackAsync(float duration)
    {
        await Awaitable.WaitForSecondsAsync(duration);

        isAttacking = false;
        weaponController.EndAttack();

        await Awaitable.WaitForSecondsAsync(duration);
        
        if (!hitController.IsDead)
        {
            StartCoroutine(StandTime(standTime));
        }
    }

    public override void OnAttackAnimation()
    {
        isAttacking = true;
        navMeshAgent.isStopped = true;
        RotateTowardsPlayer();
        lastAttackTime = Time.time;

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Regular Attack"))
        {
            AttackAsync(stateInfo.length * 0.5f);
        }

        animator.SetInteger("Atk", 0);
    }

    public override void OnAttackAnimationEnd()
    {
        //animator.SetInteger("Atk", 0);
        // isAttacking = false;
        // weaponController.EndAttack();
        // StartCoroutine(StandTime(standTime));
    }
}

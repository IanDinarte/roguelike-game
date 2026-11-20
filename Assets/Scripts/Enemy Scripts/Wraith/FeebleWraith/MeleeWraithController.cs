using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeWraithController : EnemyController
{
    private MeleeWraithAttackController attackController;

    public MeleeWraithController()
    {
        this.SetAggroRange(3f);
        this.SetDisaggroRange(15f);
        this.SetDmgAggroDuration(5f);
    }

    protected override void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        hitController = this.GetComponent<EnemyHitController>();
        attackController = this.GetComponent<MeleeWraithAttackController>();
        fow = this.GetComponentInChildren<FieldOfView>();

        spawnLocation = this.transform.position;
        spawnRotation = this.transform.rotation;

        defaultStoppingDistance = navMeshAgent.stoppingDistance;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Start()
    {
        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    protected override void Update()
    {
        if (Time.time >= lastDmgTime + dmgAggroDuration)
        {
            isDmgAggroed = false;
        }

        if (player != null && !hitController.IsDead)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (navMeshAgent.enabled)
            {
                if (!isAggroed && (canSeeTarget || distanceToPlayer <= aggroRange))
                {
                    isAggroed = true;
                    hitController.redEyes.SetActive(true);
                    navMeshAgent.stoppingDistance = defaultStoppingDistance;
                }
                else if ((isAggroed || isDmgAggroed) && distanceToPlayer <= attackController.attackRange)
                {
                    navMeshAgent.SetDestination(player.position);
                    attackController.PrepareAttack();
                }
                else if (distanceToPlayer <= disaggroRange && isAggroed || isDmgAggroed)
                {
                    navMeshAgent.SetDestination(player.position);
                }
                else if (!isAggroed && !isDmgAggroed && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    //Debug.Log("got to spawn point");
                    //transform.position = spawnLocation;
                    //transform.rotation = spawnRotation;
                    transform.rotation = Quaternion.Slerp(transform.rotation, spawnRotation, 2f * Time.deltaTime);
                }
                else if (distanceToPlayer > disaggroRange)
                {
                    isAggroed = false;
                    hitController.redEyes.SetActive(false);
                    navMeshAgent.stoppingDistance = 0f;
                    navMeshAgent.SetDestination(spawnLocation);
                }
            }
            UpdateAnimator();
        }
        currentDestination = navMeshAgent.destination;
        //currentMovement = this.rb.velocity;
    }
}

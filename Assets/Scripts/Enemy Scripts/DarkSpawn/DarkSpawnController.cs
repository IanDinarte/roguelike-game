using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DarkSpawnController : EnemyController
{
    private EnemyAttackController attackController;

    public DarkSpawnController()
    {
        this.SetAggroRange(50f);
        this.SetDisaggroRange(0f);
        this.SetDmgAggroDuration(0f);
    }

    protected override void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        hitController = this.GetComponent<EnemyHitController>();
        attackController = this.GetComponent<EnemyAttackController>();

        spawnLocation = this.transform.position;
        spawnRotation = this.transform.rotation;

        defaultStoppingDistance = navMeshAgent.stoppingDistance;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    protected override void Update()
    {
        if (player != null && !hitController.IsDead && !hitController.isInvincible)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (navMeshAgent.enabled)
            {
                if (distanceToPlayer <= attackController.attackRange)
                {
                    navMeshAgent.SetDestination(player.position);
                    attackController.PrepareAttack();
                }
                else if (distanceToPlayer <= aggroRange)
                {
                    navMeshAgent.SetDestination(player.position);
                }
            }
            UpdateAnimator();
        }
        currentDestination = navMeshAgent.destination;
    }
}

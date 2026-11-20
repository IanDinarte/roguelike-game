using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedWraithController : EnemyController
{
    private RangedWraithAttackController attackController;
    //variables
    protected float dangerDistance = 3f; //distance to start fleeing

    public RangedWraithController()
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
        attackController = this.GetComponent<RangedWraithAttackController>();
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

            if (navMeshAgent.enabled == true)
            {

                if (!isAggroed && (fow.FindVisibleTarget() || distanceToPlayer <= aggroRange))
                {
                    isAggroed = true;
                    hitController.redEyes.SetActive(true);
                }
                else if ((isAggroed || isDmgAggroed) && distanceToPlayer <= attackController.attackRange)
                {

                    if (distanceToPlayer <= dangerDistance)
                    {
                        Flee();
                    }
                    else
                    {
                        attackController.PrepareAttack();
                    }
                }
                else if (distanceToPlayer <= disaggroRange && isAggroed || isDmgAggroed)
                {
                    GetInAttackRange();
                }
                else if (!isAggroed && !isDmgAggroed && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
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
    }

    // void Flee()
    // {
    //     //Debug.Log("Im fleeing");
    //     Vector3 directionToPlayer = transform.position - player.position;
    //     Vector3 fleeDirection = directionToPlayer.normalized;

    //     // Calculate the target position within the attack range
    //     Vector3 targetPosition = transform.position + fleeDirection * attackController.attackRange;

    //     // Check if the target position is reachable
    //     NavMeshHit hit;
    //     if (NavMesh.SamplePosition(targetPosition, out hit, attackController.attackRange, NavMesh.AllAreas))
    //     {
    //         navMeshAgent.SetDestination(hit.position);
    //     }
    // }

    void Flee()
    {
        Vector3 bestFleePosition = transform.position;
        float bestDistance = 0f;

        Vector3 directionToPlayer = transform.position - player.position;
        Vector3 baseFleeDirection = directionToPlayer.normalized;

        int directionsToTry = 12; // More = better but more performance cost
        float angleStep = 180f / (directionsToTry - 1); // 180° semi-circle

        float fleeDistance = 3f;

        for (int i = 0; i < directionsToTry; i++)
        {
            float angle = -90f + angleStep * i;
            Vector3 rotatedDirection = Quaternion.Euler(0, angle, 0) * baseFleeDirection;
            Vector3 candidatePosition = transform.position + rotatedDirection * fleeDistance;

            if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (navMeshAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    float distanceFromPlayer = Vector3.Distance(hit.position, player.position);
                    if (distanceFromPlayer > bestDistance)
                    {
                        bestDistance = distanceFromPlayer;
                        bestFleePosition = hit.position;
                    }
                }
            }
        }

        if (bestDistance > 0f)
        {
            navMeshAgent.SetDestination(bestFleePosition);
        }
        else
        {
            Debug.Log("Enemy is cornered and no valid flee path was found.");
        }
    }

    void GetInAttackRange()
    {
        //Debug.Log("Im getting in range again");
        // Calculate the direction towards the player
        Vector3 directionToPlayer = player.position - transform.position;
        Vector3 approachDirection = directionToPlayer.normalized;

        // Calculate the target position at the edge of the attack range
        Vector3 targetPosition = player.position - approachDirection * attackController.attackRange;

        // Check if the target position is reachable
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, attackController.attackRange, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }
}
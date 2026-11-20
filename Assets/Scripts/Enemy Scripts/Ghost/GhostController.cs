using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//make slow reg attack
public class GhostController : EnemyController
{
    private GhostAttackController attackController;

    //Fleeing mode
    public bool isInvisible = false; //invisible while fleeing
    //public float dangerDistance = 5.0f;
    //private float wanderDuration = 3f;
    //public bool isMoving = false;
    public Vector3 wanderDestination;

    public GhostController()
    {
        this.SetAggroRange(5f);
        this.SetDisaggroRange(20f);
        this.SetDmgAggroDuration(5f);
    }

    protected override void Awake()
    {
        animator = this.GetComponent<Animator>();
        attackController = this.GetComponent<GhostAttackController>();
        hitController = this.GetComponent<EnemyHitController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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

        if (player != null && !hitController.IsDead && navMeshAgent.enabled == true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isInvisible)
            {
                Panic();
            }
            else if (!isAggroed && (fow.FindVisibleTarget() || distanceToPlayer <= aggroRange))
            {
                isAggroed = true;
            }
            else if ((isAggroed || isDmgAggroed) && attackController.canCharge && distanceToPlayer <= attackController.chargeRange
            && attackController.IsFacingPlayer() && distanceToPlayer > attackController.chargeMinRange) //charge attack
            {
                attackController.PrepareChargeAttack();
            }
            else if ((isAggroed || isDmgAggroed) && distanceToPlayer <= attackController.attackRange) //Regular attack
            {
                attackController.PrepareAttack();
            }
            else if ((isAggroed || isDmgAggroed) && distanceToPlayer <= disaggroRange)
            {
                navMeshAgent.SetDestination(player.position);
            }
            else if (!isAggroed && !isDmgAggroed && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, spawnRotation, 2f * Time.deltaTime);
            }
            else if (distanceToPlayer > disaggroRange)
            {
                isAggroed = false;
                navMeshAgent.stoppingDistance = 0f;
                navMeshAgent.SetDestination(spawnLocation);
            }
            UpdateAnimator();
        }
        currentDestination = navMeshAgent.destination;
    }

    void Panic()
    {
        //if (isMoving == false)
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            //Debug.Log("wanderin");

            Vector2 circle = Random.insideUnitCircle * 10;
            Vector3 randomDirection = new Vector3(circle.x, 0, circle.y);
            Vector3 candidatePosition = transform.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePosition, out hit, 10f, NavMesh.AllAreas))
            {
                wanderDestination = hit.position;
                navMeshAgent.SetDestination(wanderDestination);
                //isMoving = true;
                //StartCoroutine(Wander(wanderDuration));
            }
        }
    }

    private IEnumerator Wander(float wanderDuration)
    {
        yield return new WaitForSeconds(wanderDuration);
        //isMoving = false;
    }
}

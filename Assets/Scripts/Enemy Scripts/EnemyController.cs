using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    protected Animator animator;
    //protected EnemyAttackController attackController; 
    protected EnemyHitController hitController;
    protected NavMeshAgent navMeshAgent;
    public Transform player;
    protected FieldOfView fow;

    //variables
    public Vector3 currentDestination;
    //public Vector3 currentMovement;
    protected Vector3 spawnLocation;
    protected Quaternion spawnRotation;
    protected float aggroRange; //range wich the enemy will start following
    protected float disaggroRange; //range wich the enemy will stop following
    protected float dmgAggroDuration;
    protected float lastDmgTime;
    public bool isDmgAggroed = false;
    public bool isAggroed = false;
    public bool canSeeTarget = false;
    protected float defaultStoppingDistance;

    public void SetAggroRange(float aggroRange) => this.aggroRange = aggroRange;

    public void SetDisaggroRange(float disaggroRange) => this.disaggroRange = disaggroRange;

    public void SetDmgAggroDuration(float dmgAggroDuration) => this.dmgAggroDuration = dmgAggroDuration;

    protected abstract void Awake();

    protected abstract void Update();

    protected void UpdateAnimator()
    {
        bool isWalking = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isWalking);
    }

    public void DamageAggro()
    {
        isDmgAggroed = true;
        lastDmgTime = Time.time;
    }

    protected IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            canSeeTarget = fow.FindVisibleTarget();
        }
    }
}
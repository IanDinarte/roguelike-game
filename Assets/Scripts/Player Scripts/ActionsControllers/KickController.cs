using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    pro chute falta:
        - decidir se vai dar dano na vida ou nao
        - decidir se o alvo vai ser afetado por gravidade (pode cair se chutado de lugares altos) ou se vou fazer que nem no champions onde 
            a maioria dos lugares altos tem barreiras invisiveis.
*/

public class KickController : MonoBehaviour
{
    private AttackController attackController;
    private HitController player;
    public int healthDamage = 1;
    public int poiseDamage = 100;
    public float knockbackForce = 5.0f;
    public float knockbackDuration = 1f;
    public int poiseCost = 10;
    private bool isKicking = false;
    private bool hasHit = false;

    private void Awake() {
        attackController = this.GetComponentInParent<AttackController>();
        player = this.GetComponentInParent<HitController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isKicking && other.CompareTag("Enemy") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            EnemyHitController enemy = other.GetComponent<EnemyHitController>();
            //Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();
            //NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();

            if (enemy != null)
            {
                enemy.KnockBack(healthDamage + attackController.PassiveHealthDamage, poiseDamage
                    + attackController.PassivePoiseDamage, knockbackForce, knockbackDuration, this.transform);
                hasHit = true;
                /*if (enemyRigidbody != null && enemyAgent != null)
                {
                    enemyAgent.enabled = false;

                    // Calculate the knockback direction
                    Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                    knockbackDirection.y = 0;
                    knockbackDirection.Normalize();

                    enemyRigidbody.velocity = Vector3.zero; // Reset the velocity
                    enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

                    //StartCoroutine(KnockbackEnemy(enemyAgent, knockbackTarget, knockbackDuration));
                    StartCoroutine(ReEnableNavMeshAgent(enemyAgent, enemyRigidbody, knockbackDuration));
                } */
            }
        }
    }

    // private IEnumerator ReEnableNavMeshAgent(NavMeshAgent agent, Rigidbody rigidbody, float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     agent.enabled = true;
    //     rigidbody.velocity = Vector3.zero;
    //     Debug.Log("NavMeshAgent re-enabled");
    // }

    // private IEnumerator KnockbackEnemy(NavMeshAgent agent, Vector3 targetPosition, float duration)
    // {
    //     agent.enabled = false; // Disable the NavMeshAgent to allow manual movement
    //     float elapsed = 0f;

    //     Vector3 startPosition = agent.transform.position;

    //     while (elapsed < duration)
    //     {
    //         agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     agent.transform.position = targetPosition; // Ensure final position is set
    //     agent.enabled = true; // Re-enable the NavMeshAgent
    //     Debug.Log("NavMeshAgent re-enabled");
    // }

    public void StartKick()
    {
        player.lastTimeHit = Time.time;
        player.currentPoise -= poiseCost;
        player.wasHit = true;
        isKicking = true;
    }

    public void EndKick()
    {
        isKicking = false;
        hasHit = false;
    }
}

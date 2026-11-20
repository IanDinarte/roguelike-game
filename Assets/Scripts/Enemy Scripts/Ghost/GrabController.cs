using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public GhostAttackController attackController;
    //private BoxCollider hitBox;

    //damage variables
    public int damagePerTick = 10; //damage per tick
    public float damageRate = 1.0f; //1 damage tick per second
    private float currentTime = 0.0f;
    public int blockDamage = 30;

    //
    private bool isAttacking = false;
    private bool hasHit = false;

    void Awake()
    {
        attackController = this.GetComponentInParent<GhostAttackController>();
        //hitBox = this.GetComponent<>(BoxCollider);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && !hasHit)
        {
            //Debug.Log("Grabbed " + other.name);
            HitController player = other.GetComponent<HitController>();
            if (player != null && !player.HasCC() && !player.isInvincible)
            {
                hasHit = true;
                player.Stun(attackController.grabDuration);
                StartCoroutine(GrabDamage(player, attackController.grabDuration));
                attackController.GrabSuccessful();
            }
        }
        else if (isAttacking && other.CompareTag("Shield") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            HitController player = other.GetComponentInParent<HitController>();
            if (player != null)
            {
                hasHit = true;
                player.TakeDamage(0, blockDamage, true, true);
                attackController.GrabFailed();
            }
        }
        else if (isAttacking && other.CompareTag("Terrain") && !hasHit)
        {
            hasHit = true;
            attackController.GrabFailed();
        }
    }

    private IEnumerator GrabDamage(HitController target, float duration)
    {
        while (currentTime < duration)
        {
            currentTime += damageRate;
            target.TakeDamage(damagePerTick, damagePerTick, false, true);
            //Debug.Log("time: " + currentTime + " Damage: " + damagePerTick);
            yield return new WaitForSeconds(damageRate);
        }
        currentTime = 0.0f;
    }

    public void StartAttack()
    {
        //hitBox.isEnabled = true;
        isAttacking = true;
    }

    public void EndAttack()
    {
        //hitBox.isEnabled = false;
        isAttacking = false;
        hasHit = false;
    }
}

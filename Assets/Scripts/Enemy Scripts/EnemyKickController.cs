using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//unused? knockback for enemies, maybe for the giant dark elf
public class EnemyKickController : MonoBehaviour
{
    public int healthDamage = 10;
    public int poiseDamage = 0;
    private bool isAttacking = false;
    private bool hasHit = false;
    public float knockbackForce = 10.0f;
    public float knockbackDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && !hasHit)
        {
            HitController player = other.GetComponent<HitController>();

            if (player != null)
            {
                //player.KnockBack(healthDamage, poiseDamage, knockbackForce, knockbackDuration, this.transform);
                hasHit = true;

            }
        }
    }

    public void StartAttack()
    {
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        hasHit = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWraithWeaponController : EnemyWeaponController
{
    public MeleeWraithWeaponController()
    {
        this.SetDamage(15);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            HitController player = other.GetComponent<HitController>();
            if (player != null)
            {
                player.TakeDamage(damage, damage, false, false);
                hasHit = true;
            }
        }
        else if (isAttacking && other.CompareTag("Shield") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            HitController player = other.GetComponentInParent<HitController>();
            if (player != null)
            {
                player.TakeDamage(damage, damage, true, false);
                hasHit = true;
            }
        }
    }
}

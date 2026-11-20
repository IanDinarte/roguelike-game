using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWraithWeaponController : EnemyWeaponController
{
    public BigWraithWeaponController()
    {
        SetDamage(40);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && !hasHit)
        {
            HitController player = other.GetComponent<HitController>();
            if (player != null)
            {
                player.TakeDamage(damage, damage, false, false);
                hasHit = true;
            }
        }
        else if (isAttacking && other.CompareTag("Shield") && !hasHit)
        {
            HitController player = other.GetComponentInParent<HitController>();
            if (player != null)
            {
                player.TakeDamage(damage, damage, true, false);
                hasHit = true;
            }
        }
    }
}

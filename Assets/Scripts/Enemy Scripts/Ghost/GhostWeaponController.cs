using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWeaponController : EnemyWeaponController
{
    //slow variables
    public float slowDuration = 5.0f;
    public float slowStrength = 1.4f;

    public GhostWeaponController()
    {
        SetDamage(10);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            HitController player = other.GetComponent<HitController>();
            if (player != null)
            {
                player.TakeDamage(damage, damage, false, true);
                player.Slow(slowDuration, slowStrength);
                hasHit = true;
            }
        }
        else if (isAttacking && other.CompareTag("Shield") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            HitController player = other.GetComponentInParent<HitController>();
            if (player != null)
            {
                player.TakeDamage(damage, damage, true, true);
                hasHit = true;
            }
        }
    }
}

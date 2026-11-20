using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : WeaponController
{
    public CapsuleCollider capsuleCollider;

    protected override void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        capsuleCollider.enabled = false;
        this.CheckAttackController();
    }

    public override void StartAttack()
    {
        if ((IsCrit = RollCrit()) == true) {
            PlayCritEffect();
        }
        capsuleCollider.enabled = true;
        isAttacking = true;
    }

    public override void EndAttack()
    {
        capsuleCollider.enabled = false;
        isAttacking = false;
        hasHit = false;
        IsCrit = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//transformar nuam classe abstrata
public abstract class EnemyWeaponController : MonoBehaviour
{
    protected int damage;
    protected bool isAttacking = false;
    protected bool hasHit = false;
    protected CapsuleCollider capsuleCollider;

    protected void Awake()
    {
        capsuleCollider = this.GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    protected abstract void OnTriggerEnter(Collider other);

    public void StartAttack()
    {
        isAttacking = true;
        capsuleCollider.enabled = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        capsuleCollider.enabled = false;
        hasHit = false;
    }
}

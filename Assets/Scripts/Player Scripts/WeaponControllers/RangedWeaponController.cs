using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponController : WeaponController
{
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    protected float arrowForce;

    public void SetArrowForce(float newArrowForce) => this.arrowForce = newArrowForce;

    protected override void Start()
    {
        this.CheckAttackController();
        GameObject spawnPointObject = GameObject.Find("Player Arrow Spawn Point");
        arrowSpawnPoint = spawnPointObject.transform;
    }

    public void ShootArrow()
    {
        Vector3 direction = arrowSpawnPoint.forward;

        // Instantiate the arrow at the spawn point's position and rotation
        GameObject arrowInstance = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

        ProjectileController projectileController = arrowInstance.GetComponent<ProjectileController>();
        projectileController.SetDamage(GetFinalDamage());

        if (IsCrit == true)
        {
            projectileController.PlayCritEffect();
        }

        arrowInstance.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Get the Rigidbody component of the arrow for applying physics
        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();

        // Apply velocity to the arrow to make it move towards the predicted position
        rb.linearVelocity = direction * arrowForce;
    }

    public override void StartAttack()
    {
        if ((IsCrit = RollCrit()) == true) {
            PlayCritEffect();
        }
        ShootArrow();
        isAttacking = true;
    }

    public override void EndAttack()
    {
        isAttacking = false;
        hasHit = false;
        IsCrit = false;
    }
}

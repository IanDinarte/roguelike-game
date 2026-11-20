using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedWeaponController : MonoBehaviour
{
    private int damage = 10;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float arrowForce = 1f;
    public Transform player;
    public AnimationMovementController playerController;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerController = player.GetComponent<AnimationMovementController>();
    }

    public void ShootArrow()
    {
        /* GameObject arrowInstance = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        arrowInstance.GetComponent<EnemyProjectileController>().damage = this.damage;
        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
        rb.velocity = arrowSpawnPoint.up * arrowForce; */

        Vector3 targetPosition = PredictPlayerPosition();
        Vector3 direction = (targetPosition - arrowSpawnPoint.position).normalized;

        // Instantiate the arrow at the spawn point's position and rotation
        GameObject arrowInstance = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        arrowInstance.GetComponent<EnemyProjectileController>().SetDamage(damage);
        arrowInstance.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Get the Rigidbody component of the arrow for applying physics
        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();

        // Apply velocity to the arrow to make it move towards the predicted position
        rb.linearVelocity = direction * arrowForce;
    }

    /**
        Used to predict the player position to shoot while the player is moving and hit.
    */
    private Vector3 PredictPlayerPosition()
    {

        Vector3 playerPosition = player.position;
        Vector3 playerVelocity = playerController.Velocity;

        playerPosition.y += 1f; //adjust to aim at the correct height

        if (playerController.CurrentMovement == Vector3.zero)
        {
            return playerPosition;
        }

        // Calculate the time it will take for the arrow to reach the player
        float distanceToPlayer = Vector3.Distance(arrowSpawnPoint.position, playerPosition);
        float timeToTarget = distanceToPlayer / arrowForce;

        // Predict the future position of the player
        Vector3 futurePosition = playerPosition + (playerVelocity * timeToTarget);

        return futurePosition;
    }
}

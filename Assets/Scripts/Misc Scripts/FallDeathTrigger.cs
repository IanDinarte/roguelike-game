using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDeathTrigger : MonoBehaviour
{
    private float destroyDelay = 3f;

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HitController player = other.GetComponent<HitController>();
            if (player != null)
            {
                player.TakeDamage(9999, 9999, false, false);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyHitController enemy = other.GetComponent<EnemyHitController>();
            if (enemy != null)
            {
                enemy.TakeDamage(9999, 9999);
                StartCoroutine(DestroyWithDelay(other.gameObject, destroyDelay));
            }
        }
    }

    private IEnumerator DestroyWithDelay(GameObject objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(objectToDestroy);
    }
}

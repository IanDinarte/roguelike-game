using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    private Vector3 origin;

    // private void Start() {
    //     StartCoroutine(FindTargetWithDelay(0.2f));
    // }

    // private IEnumerator FindTargetWithDelay(float delay)
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(delay);
    //         FindVisibleTarget();
    //     }
    // }

    public bool FindVisibleTarget()
    {
        origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        Collider[] targetsInViewRadius = Physics.OverlapSphere(origin, viewRadius, targetMask);
        //Collider[] targetsBelowInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //Collider[] targetsInViewRadius = targetsInViewRadius.Concat(targetsBelowInViewRadius).ToArray();

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    //Debug.Log("i can see target");
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

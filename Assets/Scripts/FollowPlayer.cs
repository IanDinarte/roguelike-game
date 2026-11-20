using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // public Transform player;
    // public float x = 0f;
    // public float y = 11f;
    // public float z = -1.75f;

    // private void Awake() {
    //     if (player == null)
    //     {
    //         player = GameObject.FindGameObjectWithTag("Player").transform;
    //     }
    // }

    // // Update is called once per frame
    // void Update () {
    //     transform.position = player.transform.position + new Vector3(x, y, z);
    // }

    public Transform player;
    public HitController hitController;
    public LayerMask obstacleMask;
    // public float x = 0f;
    // public float y = 11f;
    // public float z = -1.75f;

    public Vector3 avoidPosition = new Vector3(0f, 0f, 2f);
    public Vector3 avoidRotationEuler = new Vector3(90f, 0f, 0f);
    public float avoidSpeed = 10f;
    public float smoothSpeed = 10f;
    public float rotationLerpSpeed = 5f;

    public float sphereRadius = 0.1f;


    private Quaternion defaultRotationEuler;
    private Vector3 desiredOffset = new Vector3(0f, 11f, -1.75f);

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        hitController = player.GetComponent<HitController>();
        //desiredOffset = new Vector3(x, y, z);
        defaultRotationEuler = this.transform.rotation;
    }

    private void LateUpdate()
    {
        if (!hitController.IsDead)
        {

            Vector3 desiredPosition = player.position + desiredOffset;

            Vector3 direction = desiredPosition - player.position;
            float distance = direction.magnitude;

            bool isObstructed = Physics.SphereCast(player.position, sphereRadius, direction.normalized, out RaycastHit hit, distance, obstacleMask);
            //Debug.DrawLine(player.position, transform.position, Color.white, 1.0f, true);

            if (isObstructed)
            {
                transform.position = Vector3.Lerp(transform.position, desiredPosition + avoidPosition, Time.deltaTime * avoidSpeed);

                Quaternion avoidRotation = Quaternion.Euler(avoidRotationEuler);
                transform.rotation = Quaternion.Lerp(transform.rotation, avoidRotation, Time.deltaTime * rotationLerpSpeed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

                Quaternion defaultRotation = defaultRotationEuler;
                transform.rotation = Quaternion.Lerp(transform.rotation, defaultRotation, Time.deltaTime * rotationLerpSpeed);
            }
        }
    }
}

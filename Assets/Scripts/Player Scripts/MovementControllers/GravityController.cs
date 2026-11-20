using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    private Animator animator;
    private AnimationMovementController movementController;
    // public BoxCollider boxCollider;
    private CharacterController characterController;

    //variables
    public Vector3 verticalVelocity;
    public float gravity = -0.5f;
    public float terminalVelocity = -20f;
    // public bool isFalling = false;
    public bool grounded = true;

    private void Awake()
    {
        animator = this.GetComponentInParent<Animator>();
        movementController = this.GetComponentInParent<AnimationMovementController>();
        // boxCollider = this.GetComponent<BoxCollider>();
        characterController = this.GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //grounded = characterController.isGrounded;
        HandleGravity();
    }

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        verticalVelocity.y = 0.0f;
        grounded = false;
    }

    void HandleGravity()
    {
        if (grounded)
        {
            movementController.isFalling = false;
            //animator.SetBool("Falling", false);
            //verticalVelocity.y = 0.0f;
        }
        else //if (!grounded && verticalVelocity.y <= -2.0f)
        {
            movementController.isFalling = true;
            //animator.SetBool("Falling", true);
            movementController.isMoving = false;
        }

        if (verticalVelocity.y > terminalVelocity)
        {
            verticalVelocity.y += gravity; //Mathf.Min(verticalVelocity.y - (Physics.gravity.y * Time.deltaTime), Physics.gravity.y);
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}

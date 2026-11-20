using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationMovementController : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;
    private Vector2 currentMovementInput;
    private HitController hitController;
    public Vector3 currentMovement;
    public AudioSource footStepsSound;

    private PlayerController playerController;

    //jumping variables //not gonna use jump in game //Dont remove
    /* private float gravity = -9f;
    public float jumpHeight = 0.5f;
    public bool isJumping = false; */

    //movement variables
    //int isWalkingHash;
    public bool canWalk = true;
    public bool isMoving;
    [SerializeField] private float moveSpeed = 4f;
    public bool isFalling = false;
    //public bool isGravityAffected = true;

    // Velocity tracking
    private Vector3 lastPosition;
    private Vector3 velocity;
    public Vector3 Velocity { get => velocity; set => velocity = value; }

    //
    public bool isImmobilized = false; //cant move at all //same as stun????
    public bool isRooted = false; //can rotate but cant move nor jump

    //getters & setters
    public Vector3 CurrentMovement => currentMovement;
    public float MoveSpeed => moveSpeed;


    public void SetCurrentMovement(Vector3 newCurrentMovement) => currentMovement = newCurrentMovement;
    public void SetMoveSpeed(float newMoveSpeed) => moveSpeed = newMoveSpeed;

    void Awake()
    {
        // characterController = this.GetComponent<CharacterController>();
        // animator = this.GetComponent<Animator>();
        // hitController = this.GetComponent<HitController>();
        // playerController = this.GetComponent<PlayerController>();

        // //isWalkingHash = Animator.StringToHash("isWalking");
        // //playerInput = new PlayerInput();
        // playerInput = playerController.playerInput;

        // playerInput.CharacterControls.Move.started += OnMovementInput;
        // playerInput.CharacterControls.Move.canceled += OnMovementInput;
        // playerInput.CharacterControls.Move.performed += OnMovementInput;
    }

    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        hitController = this.GetComponent<HitController>();
        playerController = this.GetComponent<PlayerController>();

        playerInput = playerController.PlayerInput;
        //isWalkingHash = Animator.StringToHash("isWalking");
        //playerInput = new PlayerInput();

        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;

        lastPosition = transform.position;
    }

    void Update()
    {
        //Debug.Log(characterController.isGrounded);
        //handleRotation();
        HandleAnimation();
        HandleFootSteps();

        if (hitController.HasCC() == false && canWalk)
        {
            playerController.CharacterController.Move(currentMovement * moveSpeed * Time.deltaTime);
        }
        //MovementJump();

        // Calculate velocity
        if (currentMovement == Vector3.zero)
        {
            velocity = currentMovement;
        }
        else
        {
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        if (!hitController.IsDead)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMoving = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }
    }

    void HandleAnimation()
    {
        /* bool isWalking = animator.GetBool(isWalkingHash);

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        } */

        float velocityX = Vector3.Dot(currentMovement.normalized, transform.right);
        float velocityZ = Vector3.Dot(currentMovement.normalized, transform.forward);

        playerController.Animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        playerController.Animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
    }

    void HandleFootSteps()
    {
        if (isMoving && hitController.HasCC() == false && canWalk)
        {
            footStepsSound.enabled = true;
        }
        else
        {
            footStepsSound.enabled = false;
        }
    }

    void OnEnable()
    {
        //playerInput.CharacterControls.Jump.started += OnJump;
        //playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //playerInput.CharacterControls.Jump.started -= OnJump;
        //playerInput.CharacterControls.Disable();
    }

    /* void OnJump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            footStepsSound.enabled = false;
            animator.SetTrigger("Jump");
            isJumping = true;
        }
    } */

    /* void MovementJump()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = 0.0f;
        }

        if (isJumping && characterController.isGrounded)
        {
            currentMovement.y += Mathf.Sqrt(jumpHeight * -1.0f * gravity);
            isJumping = false;
        }

        currentMovement.y += gravity * Time.deltaTime;
        characterController.Move(currentMovement * Time.deltaTime);
    } */
}

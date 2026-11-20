using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DodgeController : MonoBehaviour
{
    private AnimationMovementController playerMovement;
    private Animator animator;
    private AttackController attackController;
    private Camera mainCamera;
    private CharacterController characterController;
    private DefendController defendController;
    // private GravityController gravityController;
    private HitController hitController;
    private PlayerInput playerInput;
    private RotationController rotationController;

    private PlayerController playerController;

    //dodge variables
    public bool isDodging = false;
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.7f;
    public int dodgePoiseCost = 10;

    //teleport variables
    public bool isTeleport = false;
    public LayerMask obstacleMask;
    private float lastTeleportTime;
    private float teleportCooldown = 0.5f;
    private string TeleportEffectName = "Teleport_Effect";
    private Transform teleportEffectLocation;

    //debug
    private float startTime = 0.0f;
    private float finishTime = 0.0f;

    public float DodgeDistance => dodgeDistance;
    public void SetDodgeDistance(float newDodgeDistance) => dodgeDistance = newDodgeDistance;

    void Start()
    {
        playerMovement = this.GetComponent<AnimationMovementController>();
        animator = this.GetComponent<Animator>();
        attackController = this.GetComponent<AttackController>();
        mainCamera = Camera.main;
        characterController = this.GetComponent<CharacterController>();
        //gravityController = this.GetComponent<GravityController>();
        hitController = this.GetComponent<HitController>();
        defendController = this.GetComponent<DefendController>();
        //playerInput = new PlayerInput();
        playerController = this.GetComponent<PlayerController>();

        playerInput = playerController.PlayerInput;
        playerInput.CharacterControls.Dodge.started += OnDodge;

        rotationController = this.GetComponent<RotationController>();
    }

    private void Update()
    {
        // RaycastHit hit;
        // Vector3 teleportDirection = GetDirection();
        // Physics.Raycast(transform.position, teleportDirection * dodgeDistance, out hit, obstacleMask);
        // Debug.DrawRay(transform.position, teleportDirection * dodgeDistance, Color.yellow);
    }

    void OnEnable()
    {
        // playerInput.CharacterControls.Dodge.started += OnDodge;
        //playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // playerInput.CharacterControls.Dodge.started -= OnDodge;
        //playerInput.CharacterControls.Disable();
    }

    void OnDodge(InputAction.CallbackContext context)
    {
        if (!defendController.isDefending && !attackController.isAttacking
                && !hitController.HasCC() && hitController.currentPoise > 0.0f)
        {
            if (isTeleport && Time.time >= lastTeleportTime + teleportCooldown)
            {
                teleportEffectLocation = this.transform;
                Teleport();
            }
            else
            {
                if (!isTeleport && !isDodging && !playerMovement.isFalling)
                {
                    hitController.isInvincible = true;
                    animator.SetInteger("Dodge", 1);
                }
            }
        }
    }

    async void Teleport()
    {
        await PlayTeleportEffect(teleportEffectLocation);

        lastTeleportTime = Time.time;

        Vector3 teleportDirection = GetDirection();
        characterController.Move(teleportDirection * dodgeDistance);

        hitController.currentPoise -= dodgePoiseCost;
        hitController.lastTimeHit = Time.time;
        hitController.wasHit = true;
    }

    public async Awaitable PlayTeleportEffect(Transform location)
    {
        string address = "Assets/Particles/Skill effects/" + TeleportEffectName + ".prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject effectInstance = Instantiate(handle.Result, location.position, location.rotation);
                    ClearEffect(effectInstance, 2f);
                }
                else
                {
                    Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };

        await Awaitable.NextFrameAsync();
    }

    public async void ClearEffect(GameObject objectToDestroy, float delay)
    {
        await Awaitable.WaitForSecondsAsync(delay);
        Destroy(objectToDestroy);
    }

    private Vector3 GetDirection()
    {
        //DODGE MOVEMENT DIRECTION (like dark souls)
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // Transform the input direction relative to the camera's forward/right
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;

        Vector3 dodgeDirection = moveDirection != Vector3.zero ? moveDirection : transform.forward;

        return dodgeDirection;
    }

    private IEnumerator DodgeRoutine()
    {
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("IgnoreEnemy");

        float elapsedTime = 0f;

        //DODGE MOUSE DIRECTION (like ruiner)
        // Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Plane plane = new Plane(Vector3.up, transform.position); // Assuming the ground is at y = 0
        // float distance;
        // Vector3 dodgeDirection = transform.forward; // Default direction in case the raycast fails

        // if (plane.Raycast(ray, out distance))
        // {
        //     Vector3 targetPoint = ray.GetPoint(distance);
        //     dodgeDirection = (targetPoint - transform.position).normalized;
        //     dodgeDirection.y = 0; // Ensure the dodge direction is horizontal
        // }

        // forward dodge while idle
        Vector3 dodgeDirection = GetDirection();

        Quaternion targetRotation = Quaternion.LookRotation(dodgeDirection);
        transform.rotation = targetRotation;

        while (elapsedTime < dodgeDuration)
        {
            float moveStep = dodgeDistance / dodgeDuration * Time.deltaTime;
            characterController.Move(dodgeDirection * moveStep);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // playerMovement.enabled = true;
        // rotationController.enabled = true;
        // isDodging = false;
        // hitController.isInvincible = false;
        playerMovement.canWalk = true;
        //playerMovement.isGravityAffected = true;
        rotationController.enabled = true;
        isDodging = false;
        hitController.isInvincible = false;

        gameObject.layer = originalLayer;
    }

    public async void DodgeAsync()
    {
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("IgnoreEnemy");

        float elapsedTime = 0f;

        Vector3 dodgeDirection = GetDirection();

        Quaternion targetRotation = Quaternion.LookRotation(dodgeDirection);
        transform.rotation = targetRotation;

        while (elapsedTime < dodgeDuration)
        {
            float moveStep = dodgeDistance / dodgeDuration * Time.deltaTime;
            characterController.Move(dodgeDirection * moveStep);

            elapsedTime += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        if (!hitController.isDead)
        {
            hitController.isInvincible = false;
            rotationController.enabled = true;
        }

        playerMovement.canWalk = true;
        isDodging = false;

        gameObject.layer = originalLayer;
    }

    public void OnDodgeAnimationBegin()
    {
        startTime = Time.time;
        //StartCoroutine(DodgeRoutine());
        DodgeAsync();
        playerMovement.footStepsSound.enabled = false;
        //playerMovement.SetCurrentMovement(Vector3.zero);
        //playerMovement.enabled = false;
        playerMovement.canWalk = false;
        //playerMovement.isGravityAffected = false;
        rotationController.enabled = false;
        isDodging = true;

        hitController.isInvincible = true;
        hitController.currentPoise -= dodgePoiseCost;
        hitController.lastTimeHit = Time.time;
        hitController.wasHit = true;

        animator.SetInteger("Dodge", 0);
    }

    public void OnDodgeAnimationEnd()
    {
        finishTime = Time.time;
        //Debug.Log("dodge animation duration: " + (finishTime - startTime));
        //playerMovement.enabled = true;
        // playerMovement.canWalk = true;
        // playerMovement.isGravityAffected = true;
        // rotationController.enabled = true;
        // isDodging = false;
        // hitController.isInvincible = false;
        //animator.SetInteger("Dodge", 0);
    }
}

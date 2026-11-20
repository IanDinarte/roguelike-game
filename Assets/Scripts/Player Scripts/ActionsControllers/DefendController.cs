using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DefendController : MonoBehaviour
{
    private Animator animator;
    private AnimationMovementController playerMovement;
    private AttackController attackController;
    //private CharacterController characterController;
    private DodgeController dodgeController;
    private EquipmentController equipmentController;
    // private GravityController gravityController;
    private HitController hitController;
    private PlayerInput playerInput;
    private ShieldController shieldController;

    private PlayerController playerController;

    //defend variables
    public bool isDefending = false;
    public bool isButtonPressed = false;

    //charge variables
    // public bool isCharging = false;
    // public int chargePoiseCost = 15;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        playerMovement = this.GetComponent<AnimationMovementController>();
        attackController = this.GetComponent<AttackController>();
        //characterController = this.GetComponent<CharacterController>();
        dodgeController = this.GetComponent<DodgeController>();
        equipmentController = this.GetComponent<EquipmentController>();
        // gravityController = this.GetComponent<GravityController>();
        hitController = this.GetComponent<HitController>();
        //playerInput = new PlayerInput();
        playerController = this.GetComponent<PlayerController>();

        playerInput = playerController.PlayerInput;
        shieldController = this.GetComponentInChildren<ShieldController>();

        playerInput.CharacterControls.Defend.started += OnDefendInput;
        playerInput.CharacterControls.Defend.canceled += OnDefendInput;
        //playerInput.CharacterControls.Defend.performed += OnDefendInput;
    }

    void Update()
    {

    }

    //this will simply give the input state (true or false)
    void OnDefendInput(InputAction.CallbackContext context)
    {
        // if (equipmentController.getCurrentOffHand() == EquipmentController.OffHand.Shield)
        // {
        //     isButtonPressed = context.ReadValueAsButton();
        //     OnDefend();
        // }
        isButtonPressed = context.ReadValueAsButton();

        // if (shieldController == null)
        // {
        //     shieldController = this.GetComponentInChildren<ShieldController>();
        // }

        DefendRoutine();

        // if (equipmentController.CheckEquippedWeaponId("04")
        //     && !attackController.isAttacking
        //     && !dodgeController.isDodging
        //     && !playerMovement.isFalling)
        // {

        //     Debug.Log("ta vindo aqui");
        //     //OnDefend();
        // }
    }

    void OnEnable()
    {
        //playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //playerInput.CharacterControls.Disable();
    }

    private void OnDefend()
    {
        if (shieldController == null)
        {
            shieldController = this.GetComponentInChildren<ShieldController>();
        }

        if (!attackController.isAttacking && !dodgeController.isDodging && !playerMovement.isFalling)
        {
            if (isButtonPressed && !playerMovement.isFalling && !hitController.HasCC())
            {
                isDefending = true;

                animator.SetInteger("Defend", 1);
            }
            else
            {
                isDefending = false;
                playerMovement.canWalk = true;
                animator.SetInteger("Defend", 0);
                shieldController.DeactivateHitBox();
            }
        }
    }

    private async void DefendRoutine()
    {
        while (isButtonPressed && !hitController.isDead)
        {
            if (equipmentController.CheckEquippedWeaponId("04")
                && !attackController.isAttacking
                && !dodgeController.isDodging
                && !playerMovement.isFalling
                && !hitController.HasCC())
            {
                if (shieldController == null)
                {
                    shieldController = this.GetComponentInChildren<ShieldController>();
                }

                animator.SetInteger("Defend", 1);
                isDefending = true;
                playerMovement.footStepsSound.enabled = false;
                playerMovement.canWalk = false;
                shieldController.ActivateHitBox();
            }
            else if (equipmentController.CheckEquippedWeaponId("04"))
            {
                isDefending = false;
                playerMovement.canWalk = true;
                animator.SetInteger("Defend", 0);
                shieldController.DeactivateHitBox();
            }

            await Awaitable.NextFrameAsync();
        }

        if (equipmentController.CheckEquippedWeaponId("04"))
        {
            isDefending = false;
            playerMovement.canWalk = true;
            animator.SetInteger("Defend", 0);
            shieldController.DeactivateHitBox();
        }
    }

    public void OnDefendAnimation()
    {
        // playerMovement.footStepsSound.enabled = false;
        // playerMovement.canWalk = false;
        // shieldController.ActivateHitBox();
    }

    public void OnDefendAnimationBegin()
    {
        // isDefending = true;

        // playerMovement.footStepsSound.enabled = false;
        // //playerMovement.SetCurrentMovement(Vector3.zero);
        // //playerMovement.enabled = false;
        // playerMovement.canWalk = false;
        // shieldController.ActivateHitBox();
        // animator.SetInteger("Defend", 2);
    }

    public void OnDefendAnimationEnd()
    {
        // isDefending = false;

        // //playerMovement.enabled = true;
        // playerMovement.canWalk = true;
        // shieldController.DeactivateHitBox();
        // //animator.SetInteger("Defend",0);
    }
}

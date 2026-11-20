using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AttackController : MonoBehaviour
{
    private AnimationMovementController playerMovement;
    private Animator animator;
    private DefendController defendController;
    private DodgeController dodgeController;
    private EquipmentController equipmentController;
    // private GravityController gravityController;
    private HitController hitController;
    private KickController kickController;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private RotationController rotationController;
    public WeaponController weaponController;
    //private CharacterController characterController;
    //public RangedWeaponController bowController;

    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource critSound;

    //variables
    public bool isAttacking = false;
    public bool canAttack = true;

    /* public bool attack1 = false;
    public bool attack2 = false;
    public bool attack3 = false; */

    //passive damage variables
    public int passiveHealthDamage = 0;
    public int passiveDamageMultiplier = 1;
    public int passivePoiseDamage = 0;
    public float passiveCritMultiplier = 1f; //this multiplies with the crit multiplier, so base is 1
    public int passiveCritFactor = 0; //crit chance %

    //onSlain
    private float healOnSlain = 0f;

    public int PassiveHealthDamage => passiveHealthDamage;
    public int PassiveDamageMultiplier => passiveDamageMultiplier;
    public int PassivePoiseDamage => passivePoiseDamage;
    public float PassiveCritMultiplier => passiveCritMultiplier;
    public int PassiveCritFactor => passiveCritFactor;

    public float HealOnSlain { get => healOnSlain; set => healOnSlain = value; }
    public AudioSource AttackSound { get => attackSound; set => attackSound = value; }
    public AudioSource CritSound { get => critSound; set => critSound = value; }

    public void SetPassiveHealthDamage(int passiveHealthDamage) => this.passiveHealthDamage = passiveHealthDamage;
    public void SetPassiveDamageMultiplier(int passiveDamageMultiplier) => this.passiveDamageMultiplier = passiveDamageMultiplier;
    public void SetPassivePoiseDamage(int passivePoiseDamage) => this.passivePoiseDamage = passivePoiseDamage;
    public void SetPassiveCritMultiplier(float passiveCritMultiplier) => this.passiveCritMultiplier = passiveCritMultiplier;
    public void SetPassiveCritFactor(int passiveCritFactor) => this.passiveCritFactor = passiveCritFactor;

    private void Awake()
    {
    }

    void Start()
    {
        //playerInput = new PlayerInput();
        playerMovement = this.GetComponent<AnimationMovementController>();
        animator = this.GetComponent<Animator>();
        equipmentController = this.GetComponent<EquipmentController>();
        defendController = this.GetComponent<DefendController>();
        dodgeController = this.GetComponent<DodgeController>();
        // gravityController = this.GetComponent<GravityController>();
        hitController = this.GetComponent<HitController>();
        kickController = this.GetComponentInChildren<KickController>();
        rotationController = this.GetComponent<RotationController>();

        playerController = this.GetComponent<PlayerController>();

        playerInput = playerController.PlayerInput;
        // characterController = this.GetComponent<CharacterController>();
        playerInput.CharacterControls.Attack.started += OnAttack;
        playerInput.CharacterControls.Kick.started += OnKick;
    }

    // void OnEnable()
    // {
    //     //playerInput.CharacterControls.Enable();
    // }

    // void OnDisable()
    // {
    //     // playerInput.CharacterControls.Attack.started -= OnAttack;
    //     // playerInput.CharacterControls.Kick.started -= OnKick;
    //     //playerInput.CharacterControls.Disable();
    // }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (!playerMovement.isFalling
            && canAttack
            && !isAttacking
            && !defendController.isDefending
            && !hitController.HasCC()
            && !dodgeController.isDodging)
        {
            CheckWeaponController();
            // if ((equipmentController.getCurrentWeapon() == EquipmentController.WeaponType.Bow && equipmentController.getArrows() > 0)
            //     || (equipmentController.getCurrentWeapon() != EquipmentController.WeaponType.Bow))
            if ((weaponController is RangedWeaponController && equipmentController.Arrows > 0)
                || (weaponController is not RangedWeaponController))
            {
                animator.SetInteger("Atk", 1);
            }
        }
    }

    /**
        When the player changes weapons the current weaponController is set to null because it's only acquired at Start,
        to mitigate this, this method will check if the weapon controller is null and promptly acquire it so null exceptions
        dont happen.
        Used when needing to access weapon controller, like at OnAttack or AttackSound.
    */
    private bool CheckWeaponController()
    {
        if (weaponController == null)
        {
            weaponController = this.GetComponentInChildren<WeaponController>();
        }
        //Debug.Log(weaponController);
        return weaponController != null;
    }

    // private IEnumerator AttackRoutine(float duration)
    // {
    //     float elapsedTime = 0f;

    //     while (elapsedTime < duration)
    //     {
    //         elapsedTime += Time.deltaTime;

    //         yield return null;
    //     }

    //     if (!hitController.isDead)
    //     {
    //         playerMovement.canWalk = true;
    //         rotationController.enabled = true;
    //         weaponController.EndAttack();
    //         isAttacking = false;
    //         canAttack = true;
    //     }
    // }

    private async void AttackAsync(float duration)
    {
        playerMovement.footStepsSound.enabled = false;
        playerMovement.canWalk = false;
        rotationController.enabled = false;
        isAttacking = true;
        canAttack = false;
        
        await Awaitable.WaitForSecondsAsync(duration);

        if (!hitController.isDead)
        {
            rotationController.enabled = true;
        }

        playerMovement.canWalk = true;
        weaponController.EndAttack();
        isAttacking = false;
        canAttack = true;
    }

    public void OnAttackAnimation()
    {
        //Debug.Log("dmg: " + this.weaponController.getDefaultHealthDamage());
        //playerMovement.SetCurrentMovement(Vector3.zero);
        //playerMovement.enabled = false;
        

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack 1"))
        {
            // Debug.Log("on attack anim state length: " + stateInfo.length);
            // StartCoroutine(AttackRoutine(stateInfo.length));
            AttackAsync(stateInfo.length);
        }

        animator.SetInteger("Atk", 0);
        //weaponController.StartAttack();
    }

    public void PlayAttackSound()
    {
        CheckWeaponController();
        AttackSound.Play();
        weaponController.StartAttack();

        if (weaponController.IsCrit == true)
        {
            CritSound.Play();
        }

        if (weaponController is RangedWeaponController)
        {
            equipmentController.SetArrows(equipmentController.Arrows - 1);
        }
    }

    public void OnAttackAnimationEnd()
    {
        //playerMovement.enabled = true;

        //rotationController.enabled = true;
        //isAttacking = false;
        //canAttack = true;
        //Debug.Log("attack ended");
        //animator.SetInteger("Atk", 0);

        // if (equipmentController.CheckEquippedWeaponId("02")) //2h
        // {
        //     animator.SetInteger("Atk", 0);
        // }
        // else if (equipmentController.CheckEquippedWeaponId("03")) //bow
        // {
        //     animator.SetInteger("Atk", 0);
        // }
        // else
        // {
        //     weaponController.EndAttack();
        // }
    }

    void OnKick(InputAction.CallbackContext context)
    {
        if (!equipmentController.CheckEquippedWeaponId("04")
            && !playerMovement.isFalling
            && canAttack
            && !isAttacking
            && !defendController.isDefending
            && hitController.currentPoise > 0.0f
            && !hitController.HasCC())
        {
            animator.SetInteger("Kick", 1);
        }
    }

    // private IEnumerator KickRoutine(float duration)
    // {
    //     float elapsedTime = 0f;

    //     while (elapsedTime < duration)
    //     {
    //         elapsedTime += Time.deltaTime;

    //         yield return null;
    //     }

    //     if (!hitController.isDead)
    //     {
    //         playerMovement.canWalk = true;
    //         rotationController.enabled = true;
    //         kickController.EndKick();
    //         isAttacking = false;
    //         canAttack = true;
    //     }
    // }

    private async void KickAsync(float duration)
    {
        await Awaitable.WaitForSecondsAsync(duration);

        if (!hitController.isDead)
        {
            rotationController.enabled = true;
        }

        playerMovement.canWalk = true;
        kickController.EndKick();
        isAttacking = false;
        canAttack = true;
    }

    public void OnKickAnimation()
    {
        animator.SetInteger("Kick", 0);
        playerMovement.footStepsSound.enabled = false;
        //playerMovement.SetCurrentMovement(Vector3.zero);
        //playerMovement.enabled = false;
        playerMovement.canWalk = false;
        rotationController.enabled = false;
        isAttacking = true;
        canAttack = false;
        kickController.StartKick();

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);
        if (stateInfo.IsName("Kick"))
        {
            //StartCoroutine(KickRoutine(stateInfo.length));
            KickAsync(stateInfo.length);
        }
    }

    public void OnKickAnimationEnd()
    {
        // playerMovement.enabled = true;
        // playerMovement.canWalk = true;
        // rotationController.enabled = true;
        // isAttacking = false;
        // canAttack = true;
        // kickController.EndKick();
    }

    /**
        This method is supposed to be used on the enemy "Die()" method, or in any other thing that
    can be slain and you want to reward the "on Slain" effects to the player.
        Currently it will do the healOnSlain, if player has no heal on slain it will simply heal 0.
        Can be used in the future for stuff like experience, buffs, achievements, quests, etc.
    */
    public void onEnemySlain()
    {
        hitController.Heal(healOnSlain);
        //gold given automatically through here??
    }
}

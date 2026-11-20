using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private AnimationMovementController movementController;
    private AttackController attackController;
    private ConsumableController consumableController;
    private DefendController defendController;
    private DodgeController dodgeController;
    private EquipmentController equipmentController;
    private HitController hitController;
    private ItemsController itemsController;
    private KickController kickController;
    private LevelController levelController;
    private PlayerInteractionController interactionController;
    private RotationController rotationController;

    private Animator animator;
    private PlayerInput playerInput;
    private CharacterController characterController;
    
    public AnimationMovementController MovementController { get => movementController; set => movementController = value; }
    public AttackController AttackController { get => attackController; set => attackController = value; }
    public ConsumableController ConsumableController { get => consumableController; set => consumableController = value; }
    public DefendController DefendController { get => defendController; set => defendController = value; }
    public DodgeController DodgeController { get => dodgeController; set => dodgeController = value; }
    public EquipmentController EquipmentController { get => equipmentController; set => equipmentController = value; }
    public HitController HitController { get => hitController; set => hitController = value; }
    public ItemsController ItemsController { get => itemsController; set => itemsController = value; }
    public KickController KickController { get => kickController; set => kickController = value; }
    public LevelController LevelController { get => levelController; set => levelController = value; }
    public PlayerInteractionController InteractionController { get => interactionController; set => interactionController = value; }
    public RotationController RotationController { get => rotationController; set => rotationController = value; }

    public Animator Animator { get => animator; set => animator = value; }
    public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }
    public CharacterController CharacterController { get => characterController; set => characterController = value; }

    void Awake()
    {
        movementController = GetComponent<AnimationMovementController>();
        attackController = GetComponent<AttackController>();
        consumableController = GetComponent<ConsumableController>();
        defendController = GetComponent<DefendController>();
        dodgeController = GetComponent<DodgeController>();
        equipmentController = GetComponent<EquipmentController>();
        hitController = GetComponent<HitController>();
        itemsController = GetComponent<ItemsController>();
        kickController = GetComponentInChildren<KickController>();
        levelController = GetComponent<LevelController>();
        interactionController = GetComponent<PlayerInteractionController>();
        rotationController = GetComponent<RotationController>();

        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        EnableInputWithDelay();
    }
    
    private async void EnableInputWithDelay() {
        await Awaitable.WaitForSecondsAsync(1f);

        PlayerInput.CharacterControls.Enable();
    }
}
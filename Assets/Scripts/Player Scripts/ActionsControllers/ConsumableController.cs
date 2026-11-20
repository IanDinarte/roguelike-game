using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ConsumableController : MonoBehaviour
{
    private PlayerInput playerInput;
    private HitController hitController;
    private AttackController attackController;
    private DefendController defendController;
    private Animator animator;
    public GameObject healingPotion;
    private GameObject rightHandItem;
    private TextMeshProUGUI potionCounterText;

    private PlayerController playerController;

    //variables
    public bool isDrinking;
    private float healRate = 0.0005f; //the lower the faster
    private float lastHeal;
    public int numberOfPotions = 5;

    public float healAmount = 50f;
    public float healPerTick;
    public float healTracking = 0.0f;

    public int Potions => this.numberOfPotions;

    public void SetPotions(int numberOfPotions)
    {
        this.numberOfPotions = numberOfPotions;
        UpdatePotionCounter();
    }

    private void Start()
    {
        GameObject potionTextObject = GameObject.Find("PotionsCounterText");
        if (potionTextObject != null)
        {
            potionCounterText = potionTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("PotionCounterText object not found in the scene!");
        }

        //playerInput = new PlayerInput();
        playerController = this.GetComponent<PlayerController>();

        playerInput = playerController.PlayerInput;
        playerInput.CharacterControls.Consumable.started += OnConsumable;

        hitController = this.GetComponent<HitController>();
        attackController = this.GetComponent<AttackController>();
        defendController = this.GetComponent<DefendController>();
        animator = this.GetComponent<Animator>();

        healPerTick = healAmount / 2.0f;

        rightHandItem = GameObject.Find("Right Hand Item");

        UpdatePotionCounter();
    }

    void OnEnable()
    {
        // playerInput.CharacterControls.Consumable.started += OnConsumable;
        //playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // playerInput.CharacterControls.Consumable.started -= OnConsumable;
        //playerInput.CharacterControls.Disable();
    }

    /**
        Updates the potion counter on the UI.
    */
    private void UpdatePotionCounter()
    {
        if (potionCounterText != null)
        {
            potionCounterText.text = "x " + numberOfPotions.ToString();
        }
    }

    void OnConsumable(InputAction.CallbackContext context)
    {
        if (numberOfPotions > 0 && hitController.currentHealth < hitController.maxHealth)
        {
            if (!isDrinking && !attackController.isAttacking && !defendController.isDefending && !hitController.HasCC())
            {
                animator.SetBool("IsDrinking", true);
                hitController.currentHealing = hitController.currentHealth + healAmount;
                //Debug.Log("Im Drinking");
                isDrinking = true;
                numberOfPotions--;

                UpdatePotionCounter();

                HealOverTime();
            }
        }
    }

    /**
        Simply enables the potion on the right hand of the player model to be visible while drinking.
    */
    public void EquipPotion()
    {
        if (rightHandItem != null)
        {
            rightHandItem.SetActive(false);
        }
        healingPotion.SetActive(true);
    }

    /**
        Simply disables the potion on the right hand of the player model.
    */
    public void UnequipPotion()
    {
        healingPotion.SetActive(false);
        if (rightHandItem != null)
        {
            rightHandItem.SetActive(true);
        }
    }

    /**
        Will continuously heal the player while he's drinking the potion, and the total heal amount wasnt reached.
        Will stop if the player is hit with a hard cc like stun or stagger, if it already healed to the max health
        and if the player does any other action like attacking, defending or dodging.
    */
    private async void HealOverTime() //test if continue when dead and dodge
    {
        EquipPotion();

        while (isDrinking && healTracking < healAmount && !attackController.isAttacking
        && !defendController.isDefending && hitController.currentHealth < hitController.maxHealth
        && !playerController.DodgeController.isDodging && !hitController.IsDead && !hitController.HasCC())
        {
            hitController.Heal(healPerTick * Time.deltaTime);
            healTracking += healPerTick * Time.deltaTime;
            await Awaitable.WaitForSecondsAsync(healRate);
        }

        // Reset after healing is done
        healTracking = 0.0f;
        hitController.currentHealing = 0.0f;
        animator.SetBool("IsDrinking", false);
        isDrinking = false;

        await Awaitable.WaitForSecondsAsync(0.5f); //drink to idle animation time roughly
        UnequipPotion();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private Animator animator;
    private AnimationMovementController movementController;
    private AttackController attackController;
    private HitController hitController;
    private PlayerInteractionController interactionController;

    //variables
    private int level;
    private int attributesCost = 100;
    private int attributesCostIncrease = 50;

    //STR
    private int strength; //damage & crit multiplier
    private int dmgIncrease = 5;
    private float critMultIncrease = 0.05f;
    private string strDesc;

    //CON
    private int constitution = 0; //max hp & tenacity
    private int maxHealthIncrease = 10;
    private float tenacityIncrease = 0.05f;
    private string conDesc;

    //VIG
    private int vigor = 0; //max poise & poise regen
    private int poiseIncrease = 3;
    private float poiseRegenIncrease = 2.5f;
    private string vigDesc;

    //AGI
    private int agility = 0; //move speed & dodgedistance?
    private float moveSpeedIncrease = 0.05f;
    private float evasionChanceIncrease = 1f;
    private string agiDesc;

    //LUK
    private int luck = 0; //crit chance & gold increase
    private int critChanceIncrease = 2;
    private float goldMultIncrease = 0.05f; //precent increase
    private string lukDesc;

    public int AttributesCost => attributesCost;

    public string StrDesc => strDesc;
    public string ConDesc => conDesc;
    public string VigDesc => vigDesc;
    public string AgiDesc => agiDesc;
    public string LukDesc => lukDesc;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        movementController = this.GetComponent<AnimationMovementController>();
        attackController = this.GetComponent<AttackController>();
        hitController = this.GetComponent<HitController>();
        interactionController = this.GetComponent<PlayerInteractionController>();
    }

    private void Start()
    {
        UpdateDescriptions();
    }

    private void UpdateDescriptions()
    {
        strDesc = "Current Level: " + strength + "\nIncreases Damage by " + dmgIncrease
            + " and Critical Damage Multiplier by " + (critMultIncrease * 100) + "% per level.";

        conDesc = "Current Level: " + constitution + "\nIncreases Max Health by " + maxHealthIncrease
            + " and Tenacity by " + (tenacityIncrease * 100) + "% per level.";

        vigDesc = "Current Level: " + vigor + "\nIncreases Max Poise by " + poiseIncrease
            + " and Poise Regeneration by " + poiseRegenIncrease + " per level.";

        agiDesc = "Current Level: " + agility + "\nIncreases Movement Speed by " + moveSpeedIncrease
            + " and Evasion Chance by " + evasionChanceIncrease + "% per level.";

        lukDesc = "Current Level: " + luck + "\nIncreases Critical Hit chance by " + critChanceIncrease
            + "% and Gold Multiplier by " + (goldMultIncrease * 100) + "% per level.";
    }

    private void LevelUp()
    {
        level += 1;
        attributesCost += attributesCostIncrease;

        UpdateDescriptions();
    }

    public void IncreaseStrength(int quantity)
    {
        attackController.SetPassiveHealthDamage(attackController.PassiveHealthDamage + (dmgIncrease * quantity));
        attackController.SetPassivePoiseDamage(attackController.PassivePoiseDamage + (int)(dmgIncrease * 0.8f * quantity));
        attackController.SetPassiveCritMultiplier(attackController.PassiveCritMultiplier + (critMultIncrease * quantity));

        strength += quantity;
        LevelUp();
    }

    public void IncreaseConstitution(int quantity)
    {
        hitController.SetMaxHealth(hitController.MaxHealth + (maxHealthIncrease * quantity));
        hitController.SetTenacity(hitController.Tenacity - (tenacityIncrease * quantity));

        hitController.currentHealth += maxHealthIncrease * quantity;

        constitution += quantity;
        LevelUp();
    }

    public void IncreaseVigor(int quantity)
    {
        hitController.SetMaxPoise(hitController.MaxPoise + (poiseIncrease * quantity));
        hitController.SetPoiseRegen(hitController.PoiseRegen + (poiseRegenIncrease * quantity));

        vigor += quantity;
        LevelUp();
    }

    public void IncreaseAgility(int quantity)
    {
        movementController.SetMoveSpeed(movementController.MoveSpeed + (moveSpeedIncrease * quantity));
        movementController.footStepsSound.pitch += moveSpeedIncrease * quantity / 4f; //4 é o mov speed base, pra ser equivalente
        animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") + moveSpeedIncrease * quantity / 4f);

        hitController.SetEvasionChance(hitController.EvasionChance + (evasionChanceIncrease * quantity));

        agility += quantity;
        LevelUp();
    }

    public void IncreaseLuck(int quantity)
    {
        attackController.SetPassiveCritFactor(attackController.PassiveCritFactor + (critChanceIncrease * quantity));
        interactionController.SetGoldMultiplier(interactionController.GoldMultiplier + (goldMultIncrease * quantity));

        luck += quantity;
        LevelUp();
    }


}

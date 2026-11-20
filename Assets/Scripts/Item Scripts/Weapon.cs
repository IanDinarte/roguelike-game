using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    [SerializeField] private string weaponType;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float weight;
    [SerializeField] private RuntimeAnimatorController weaponAnimator;
    [SerializeField] private AudioClip attackSound;

    [Header("Weapon Stats")]
    [SerializeField] private int baseHealthDamage;
    [SerializeField] private int basePoiseDamage;
    [SerializeField] private float critMultiplier;
    [SerializeField] private int critFactor;
    [SerializeField] private float arrowForce;

    public string WeaponType => weaponType;
    public float AttackSpeed => attackSpeed;
    public float Weight => weight;
    public RuntimeAnimatorController WeaponAnimator => weaponAnimator;
    public AudioClip AttackSound => attackSound;

    public int BaseHealthDamage => baseHealthDamage;
    public int BasePoiseDamage => basePoiseDamage;
    public float CritMultiplier => critMultiplier;
    public int CritFactor => critFactor;
    public float ArrowForce => arrowForce;

    public void SetWeaponType(string newType) => weaponType = newType;
    public void SetAttackSpeed(float newAttackSpeed) => attackSpeed = newAttackSpeed;
    public void SetWeight(float newWeight) => weight = newWeight;
    public void SetWeaponAnimator(RuntimeAnimatorController newAnimator) => weaponAnimator = newAnimator;
    public void SetAttackSound(AudioClip newAttackSound) => attackSound = newAttackSound;

    public void SetBaseHealthDamage(int newBaseHealthDamage) => baseHealthDamage = newBaseHealthDamage;
    public void SetBasePoiseDamage(int newBasePoiseDamage) => basePoiseDamage = newBasePoiseDamage;
    public void SetCritMultiplier(float newCritMultipler) => critMultiplier = newCritMultipler;
    public void SetCritFactor(int newCritFactor) => critFactor = newCritFactor;
    public void SetArrowForce(float newArrowForce) => arrowForce = newArrowForce;

    private void OnEnable()
    {
        if (baseHealthDamage != 0)
        {
            SetItemStats("Dmg: " + baseHealthDamage + " / Stagger: " + basePoiseDamage + "\nCrit: " + critFactor + "% / " + critMultiplier + "x");
        }
        else
        {
            SetItemStats("");
        }
    }
}

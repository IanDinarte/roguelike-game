using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindWitchHitController : BossHitController
{
    private MindWitchController controller;
    private CapsuleCollider capsuleCollider;

    public bool isInvincible;

    public bool IsInvincible => isInvincible;

    public void SetIsInvincible(bool state) => isInvincible = state;

    public MindWitchHitController()
    {
        this.SetBossName("Mind Witch");
        this.SetMaxHealth(2000);
        this.SetCurrentHealth(2000);
        this.SetMaxPoise(500);
        this.SetCurrentPoise(500);
    }

    private void Awake()
    {
        if (healthBar == null)
        {
            healthBar = GameObject.Find("Boss Health Bar");
        }
        capsuleCollider = this.GetComponent<CapsuleCollider>();
        controller = this.GetComponent<MindWitchController>();
        animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentPoise = maxPoise;

        playerAttack = controller.player.gameObject.GetComponent<AttackController>();
    }

    private void Update()
    {
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public override void TakeDamage(int healthDamage, int poiseDamage)
    {
        if (!isDead && !isInvincible)
        {
            currentHealth -= healthDamage;
            currentPoise -= poiseDamage;
            hitSound.Play();
            bloodSplatter.Play();
            if (currentPoise <= 0 && !isStaggered)
            {
                isStaggered = true;
                animator.SetInteger("Stagger", 1);
                controller.Interrupt();
                currentPoise = maxPoise;
            }
        }
    }

    public void OnStaggerAnimation()
    {
        animator.SetInteger("Stagger", 0);
    }

    public void OnStaggerAnimationEnd()
    {
        isStaggered = false;
    }

    protected override void Die()
    {
        playerAttack.onEnemySlain();

        capsuleCollider.enabled = false;
        controller.Interrupt();
        controller.SetCanRotate(false);
        animator.SetTrigger("Die");
        isDead = true;
    }
}
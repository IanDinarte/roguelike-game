using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossHitController : MonoBehaviour
{
    public Animator animator;
    public AudioSource hitSound;
    public ParticleSystem bloodSplatter;
    public GameObject healthBar;
    protected AttackController playerAttack;

    protected string bossName;

    protected int maxHealth;
    public int currentHealth;
    protected int maxPoise;
    public int currentPoise;

    protected int currentHealing;

    protected bool isDead = false;
    protected bool isStaggered = false;

    public string BossName => bossName;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxPoise => maxPoise;
    public int CurrentPoise => currentPoise;

    public int CurrentHealing => currentHealing;

    public bool IsDead => isDead;
    public bool IsStaggered => isStaggered;

    public void SetBossName(string newName) => bossName = newName;

    public void SetMaxHealth(int newMaxHealth) => maxHealth = newMaxHealth;
    public void SetCurrentHealth(int newCurrentHealth) => currentHealth = newCurrentHealth;
    public void SetMaxPoise(int newMaxPoise) => maxPoise = newMaxPoise;
    public void SetCurrentPoise(int newCurrentPoise) => currentPoise = newCurrentPoise;

    public abstract void TakeDamage(int healthDamage, int poiseDamage);

    protected abstract void Die();
}
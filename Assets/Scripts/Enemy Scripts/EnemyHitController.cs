using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyHitController : MonoBehaviour
{
    protected Animator animator;
    protected CapsuleCollider capsuleCollider;
    //public BoxCollider boxCollider;
    protected NavMeshAgent navMeshAgent;
    protected Rigidbody rigidBody;
    public ParticleSystem bloodSplatter;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource hitGrunt;
    [SerializeField] private AudioSource deathSound;
    public Transform rightHand;
    public Transform leftHand;
    public GameObject redEyes;
    public ParticleSystem blackSludge;
    protected EnemyLootController lootController;
    protected AttackController playerAttack;

    //variables
    public int maxHealth = 100;
    public int health = 100;
    public int maxPoise = 100;
    public int poise = 100;
    [SerializeField] private bool isDead = false;
    public bool knocked = false;
    public bool isStaggered = false;
    public bool isInvincible = false;

    public bool IsDead { get => isDead; set => isDead = value; }
    public AudioSource HitSound { get => hitSound; set => hitSound = value; }
    public AudioSource HitGrunt { get => hitGrunt; set => hitGrunt = value; }
    public AudioSource DeathSound { get => deathSound; set => deathSound = value; }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public int GetMaxPoise()
    {
        return maxPoise;
    }

    public void SetMaxPoise(int maxPoise)
    {
        this.maxPoise = maxPoise;
    }

    public int GetPoise()
    {
        return poise;
    }

    public void SetPoise(int poise)
    {
        this.poise = poise;
    }

    protected abstract void Awake();

    public abstract void TakeDamage(int healthDamage, int poiseDamage);

    // public abstract void KnockBack(int healthDamage, int poiseDamage, float knockbackForce, float knockbackDuration, Transform source);

    // protected abstract IEnumerator WaitForKnockback(float delay);

    // public abstract void OnKnockedAnimation();

    // public abstract void OnGetUpAnimation();

    // public abstract void OnGetUpAnimationEnd();

    public abstract void KnockBack(int healthDamage, int poiseDamage, float knockbackForce, float knockbackDuration, Transform source);

    public abstract void OnHitAnimationEnd();

    public void ClearHands()
    {
        ClearItems(rightHand);
        ClearItems(leftHand);
    }

    public void ClearItems(Transform parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    protected abstract void Die();

    protected abstract void OnDeathAnimation();
}

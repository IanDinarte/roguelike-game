using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix name convention
public abstract class WeaponController : MonoBehaviour
{
    public AttackController attackController;

    [SerializeField] private Renderer weaponMaterial;
    private Color critEffectColor = new Color32(60, 0, 191, 255);
    private float critEffectFadeDuration = 0.5f;

    private int defaultHealthDamage;
    private int defaultPoiseDamage;
    private float critMultiplier;
    private int critFactor;
    private bool isCrit = false;
    public bool isAttacking = false;
    public bool hasHit = false;

    protected abstract void Start();

    public int DefaultHealthDamage => defaultHealthDamage;
    public int DefaultPoiseDamage => defaultPoiseDamage;
    public float CritMultiplier => critMultiplier;
    public int CritFactor => critFactor;

    public bool IsCrit { get => isCrit; set => isCrit = value; }

    public void SetDefaultHealthDamage(int defaultHealthDamage) => this.defaultHealthDamage = defaultHealthDamage;
    public void SetDefaultPoiseDamage(int defaultPoiseDamage) => this.defaultPoiseDamage = defaultPoiseDamage;
    public void SetCritMultiplier(float critMultiplier) => this.critMultiplier = critMultiplier;
    public void SetCritFactor(int critFactor) => this.critFactor = critFactor;

    public int[] GetFinalDamage()
    {
        return CalculateFinalDamage(IsCrit);
    }

    protected void CheckAttackController()
    {
        if (attackController == null)
        {
            attackController = this.GetComponentInParent<AttackController>();
        }
    }


    private int[] CalculateFinalDamage(bool isCrit)
    {
        int[] finalResult = new int[2];

        finalResult[0] = (defaultHealthDamage + attackController.PassiveHealthDamage) * attackController.PassiveDamageMultiplier;
        finalResult[1] = (defaultPoiseDamage + attackController.PassivePoiseDamage) * attackController.PassiveDamageMultiplier;

        if (isCrit)
        {
            finalResult[0] = (int)(finalResult[0] * (critMultiplier * attackController.PassiveCritMultiplier)); //hp dmg
            finalResult[1] = (int)(finalResult[1] * (critMultiplier * attackController.PassiveCritMultiplier)); //poise dmg
            Debug.Log("Crit! default dmg: " + defaultHealthDamage + "/" + defaultPoiseDamage + "\nfinal dmg: " + finalResult[0] + "/" + finalResult[1]);
            return finalResult;
        }
        //Debug.Log("default dmg: " + defaultHealthDamage + "/" + defaultPoiseDamage + "\nfinal dmg: " + finalResult[0] + "/" + finalResult[1]);
        //Debug.Log("Crit chance: " + (critFactor + attackController.PassiveCritFactor));
        return finalResult;
    }

    protected bool RollCrit()
    {
        bool isCrit = (Random.Range(1, 101) + critFactor + attackController.PassiveCritFactor) > 99;

        return isCrit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Enemy") && !hasHit)
        {
            //Debug.Log("Hit " + other.name);
            EnemyHitController enemy = other.GetComponent<EnemyHitController>();
            if (enemy != null)
            {
                int[] damage = GetFinalDamage();
                enemy.TakeDamage(damage[0], damage[1]);
                hasHit = true;
            }
        }
        else if (isAttacking && other.CompareTag("Boss") && !hasHit)
        {
            BossHitController boss = other.GetComponent<BossHitController>();
            if (boss != null)
            {
                int[] damage = GetFinalDamage();
                boss.TakeDamage(damage[0], damage[1]);
                hasHit = true;
            }
        }
        else if (isAttacking && other.CompareTag("Destructible") && !hasHit)
        {
            //play destruction sound or whatever
            Destroy(other.gameObject);
        }
    }

    protected async void PlayCritEffect()
    {
        float elapsedTime = 0f;
        float intensity = 0f;

        weaponMaterial.material.EnableKeyword("_EMISSION");

        while (elapsedTime < (critEffectFadeDuration * 0.5f))
        {
            elapsedTime += Time.deltaTime;
            intensity += 10 / (critEffectFadeDuration * 0.5f) * Time.deltaTime;

            weaponMaterial.material.SetColor("_EmissionColor", critEffectColor * intensity);

            await Awaitable.NextFrameAsync();
        }

        elapsedTime = 0f;

        while (elapsedTime < (critEffectFadeDuration * 0.5f))
        {
            elapsedTime += Time.deltaTime;
            intensity -= 10 / (critEffectFadeDuration * 0.5f) * Time.deltaTime;

            weaponMaterial.material.SetColor("_EmissionColor", critEffectColor * intensity);

            await Awaitable.NextFrameAsync();
        }

        weaponMaterial.material.SetColor("_EmissionColor", critEffectColor * -10);

        weaponMaterial.material.DisableKeyword("_EMISSION");
    }

    public abstract void StartAttack();

    public abstract void EndAttack();
}

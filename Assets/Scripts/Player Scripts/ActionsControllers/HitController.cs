using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.SceneManagement;

public class HitController : MonoBehaviour
{
    private GameManager gameManager;
    private AnimationMovementController playerMovement;
    private Animator animator;
    private AttackController attackController;
    private CharacterController characterController;
    private DefendController defendController;
    private DodgeController dodgeController;
    private ItemsController itemsController;
    private PlayerController playerController;
    private RotationController rotationController;

    //light stuff
    private Light playerLight;
    private Transform lightTransform;
    private Vector3 lightPosition;
    private Color defaultLightColor = new Color32(255, 202, 125, 255);
    private Color lowHealthLightColor = new Color32(175, 0, 0, 255);
    private Color currentLightColor;
    private float lightColorFadeSpeed = 0.5f;

    //vfx and sfx
    public AudioSource hitSound;
    public ParticleSystem bloodSplatter;
    public Transform effectLocation;
    private string slowEffectName = "Slow_Effect";
    private string stunEffectName = "Stun_Effect";

    //health variables
    public int maxHealth = 100;
    public float currentHealth;
    public bool isDead = false;
    public float currentHealing;
    //private bool isPassiveRegen = false;
    public bool isDeathProtected = false;
    private int lives = 5;
    private TextMeshProUGUI livesCounter;
    private Vector3 spawnPoint;

    private BlackScreen blackScreen;

    //poise variables
    public int maxPoise = 30;
    public float currentPoise;
    private float poiseRegenRate = 0.0001f; //the lower the faster //it's awkward but ignore, it works.
    public float poiseRegenPerTick = 10.0f;
    public bool isRegeneratingPoise = false;
    private float timeBeforePoiseRegen = 3.0f;
    public float lastTimeHit;
    public bool wasHit = false;

    //hard cc
    public float tenacity = 1f;
    private int originalLayer;
    public bool isStaggered = false;
    public bool isStunned = false;

    //soft cc
    public float slowRemainingTime = 0f;
    private Coroutine slowCoroutine;
    private float newSlowStrength;

    private float blindRemainingTime = 0f;
    private Coroutine blindCoroutine;


    //defense variables
    public float armorValue = 0f;
    public float magicResistValue = 0f;
    public float evasionChance = 0f;
    public bool isInvincible = false;
    private float gracePeriod = 5f;

    //getters & setters
    public int MaxHealth => maxHealth;
    public int Lives => lives;
    public int MaxPoise => maxPoise;
    public float PoiseRegen => poiseRegenPerTick;
    public float Tenacity => tenacity;
    public float ArmorValue => armorValue;
    public float MagicResistValue => magicResistValue;
    public float EvasionChance => evasionChance;
    public Transform LightTransform => lightTransform;
    public Vector2 LightAngle => new Vector2(playerLight.innerSpotAngle, playerLight.spotAngle);
    public bool IsDead => isDead;

    public void SetMaxHealth(int newMaxHealth) => maxHealth = newMaxHealth;
    public void SetSpawnPoint(Vector3 newSpawnPoint) => spawnPoint = newSpawnPoint;
    public void SetLightPosition(Vector3 newLightPosition) => lightPosition = newLightPosition;
    public void SetMaxPoise(int newMaxPoise) => maxPoise = newMaxPoise;
    public void SetPoiseRegen(float newPoiseRegen) => poiseRegenPerTick = newPoiseRegen;
    public void SetTenacity(float newTenacity) => tenacity = newTenacity;
    public void SetArmorValue(float newArmorValue) => armorValue = newArmorValue;
    public void SetMagicResistValue(float newMagicResistValue) => magicResistValue = newMagicResistValue;
    public void SetEvasionChance(float newEvasionChance) => evasionChance = newEvasionChance;
    public void SetLightAngle(Vector2 newLightAngle)
    {
        playerLight.spotAngle = newLightAngle.y;
        playerLight.innerSpotAngle = newLightAngle.x;
    }

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        playerController = GetComponent<PlayerController>();

        characterController = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        playerMovement = this.GetComponent<AnimationMovementController>();
        rotationController = this.GetComponent<RotationController>();
        attackController = this.GetComponent<AttackController>();
        defendController = this.GetComponent<DefendController>();
        dodgeController = this.GetComponent<DodgeController>();
        itemsController = this.GetComponent<ItemsController>();
        playerLight = this.GetComponentInChildren<Light>();


        originalLayer = gameObject.layer;

        spawnPoint = this.transform.position;
        lightTransform = this.transform.Find("Area Light").transform;
        lightPosition = lightTransform.position;

        blackScreen = GameObject.Find("BlackScreen").GetComponent<BlackScreen>();

        //currentHealth = maxHealth;
        currentPoise = maxPoise;
    }

    private void Start()
    {
        currentLightColor = playerLight.color;

        livesCounter = GameObject.Find("LivesCounter").GetComponent<TextMeshProUGUI>(); ;
        if (livesCounter != null)
        {
            livesCounter.text = Utils.ToRoman(lives);
        }
    }

    void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentPoise > maxPoise)
        {
            currentPoise = maxPoise;
        }

        if (lives == 1)
        {
            gameManager.Rot = true;
        }
        else if (gameManager.Rot == true && lives > 1)
        {
            gameManager.Rot = false;
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }

        if (currentPoise < 0.0f)
        {
            currentPoise = 0.0f;
        }
        else if (currentPoise == 0.0f && isStaggered)
        {
            wasHit = false;
        }

        if (Time.time >= lastTimeHit + timeBeforePoiseRegen)
        {
            wasHit = false;
        }

        if (!wasHit && isRegeneratingPoise == false)
        {
            isRegeneratingPoise = true;
            StartCoroutine(PoiseRegeneration());
        }
    }

    public void StartPassiveHealthRegen(float regenRate)
    {
        //isPassiveRegen = true;
        StartCoroutine(PassiveHealthRegen(regenRate));
    }

    private IEnumerator PassiveHealthRegen(float regenRate)
    {
        //Debug.Log("regening");
        while (!isDead && itemsController.GetItemCount() != 0 && itemsController.HasItem("02"))
        {
            currentHealth += regenRate;
            yield return new WaitForSeconds(1);
        }

        //isPassiveRegen = false;
    }

    private IEnumerator PoiseRegeneration()
    {
        while (currentPoise < maxPoise && !wasHit)
        {
            currentPoise += poiseRegenPerTick * Time.deltaTime;
            yield return new WaitForSeconds(poiseRegenRate);
        }

        isRegeneratingPoise = false;
    }

    public void Heal(float healingValue)
    {
        if (!isDead)
        {
            currentHealth += healingValue;// * Time.deltaTime;
            if (currentHealth <= maxHealth * 0.5)
            {
                FadeLightColorToDefault(lightColorFadeSpeed);
            }
            else if (currentHealth > maxHealth * 0.5)
            {
                playerLight.color = defaultLightColor;
                currentLightColor = playerLight.color;
            }
        }
    }

    public void IncreaseMaxHealth(int value)
    {
        if (!isDead)
        {
            maxHealth += value;
            currentHealth += value;
        }
    }

    /**
        The amount to reduce from the damage is equal to 60% of the armor/mr value,
    this will then reduce a percentage of the damage received.
        Example: You have 50 armor, this will equal 30% damage reduction.
        The max cap is 60% damage reduction at 100 armor.
    */
    private float DamageMitigationPercentage(bool isMagicDamage)
    {
        float mitigationPercentage = (float)((isMagicDamage ? magicResistValue : armorValue) * 0.6) / 100;

        if (mitigationPercentage > 0.6f)
        {
            return mitigationPercentage = 0.6f;
        }

        return mitigationPercentage;
    }

    private bool RollEvasion()
    {
        bool evade = (Random.Range(1, 101) + evasionChance) > 100;

        return evade;
    }

    public void TakeDamage(int healthDamage, int poiseDamage, bool blocked, bool isMagicDamage)
    {
        if (!isDead && !isInvincible && !RollEvasion())
        {
            wasHit = true;

            healthDamage -= (int)(healthDamage * DamageMitigationPercentage(isMagicDamage)); //mitigate the damage based on armor value before dealing damage
            lastTimeHit = Time.time;
            if (blocked)
            {
                currentPoise -= poiseDamage / 3;
                int excessDamage = (int)currentPoise * (-1);

                if (currentPoise <= 0 && !isStaggered)
                {
                    if (excessDamage > 0)
                    {
                        currentHealth -= excessDamage;
                        if (currentHealing > 0.0f) { currentHealing -= excessDamage; }
                        hitSound.Play();
                        bloodSplatter.Play();

                        if (currentHealth <= maxHealth * 0.5)
                        {
                            FadeLightColorToRed(lightColorFadeSpeed);
                        }
                    }
                    /* playerMovement.enabled = false;
                    isStaggered = true; */
                    //animator.SetFloat("StaggerMultiplier", 0.3f);
                    if (!isDead)
                    {
                        animator.SetInteger("Hit", 1);
                    }
                    defendController.isDefending = false;
                }
                else
                {
                    //playerMovement.enabled = false;
                    //animator.SetTrigger("DefendImpact");
                    animator.SetInteger("Defend", 2);
                }
            }
            else
            {
                currentHealth -= healthDamage;
                if (currentHealing > 0.0f) { currentHealing -= healthDamage; }
                currentPoise -= poiseDamage;
                if (healthDamage > 0)
                {
                    hitSound.Play();
                    bloodSplatter.Play();

                    if (currentHealth <= maxHealth * 0.5)
                    {
                        FadeLightColorToRed(lightColorFadeSpeed);
                    }

                    if (!playerMovement.isFalling && currentPoise <= 0 && !HasCC() && !isDead) //!attackController.isAttacking && 
                    {
                        /* playerMovement.enabled = false;
                        isStaggered = true; */
                        //defendController.enabled = false;
                        animator.SetInteger("Hit", 1);
                        //lastStaggerTime = Time.time;
                    }
                }
            }
        }
    }

    private async void FadeLightColorToRed(float lightColorFadeSpeed)
    {
        float elapsedTime = 0f;
        float percent = currentHealth / (maxHealth * 0.5f);
        Color targetColor = Color.Lerp(lowHealthLightColor, defaultLightColor, percent);

        while (elapsedTime < lightColorFadeSpeed)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / lightColorFadeSpeed);

            playerLight.color = Color.Lerp(currentLightColor, targetColor, t);

            await Awaitable.NextFrameAsync();
        }

        currentLightColor = playerLight.color;
    }

    private async void FadeLightColorToDefault(float lightColorFadeSpeed)
    {
        float elapsedTime = 0f;
        float percent = currentHealth / (maxHealth * 0.5f);
        Color targetColor = Color.Lerp(lowHealthLightColor, defaultLightColor, percent);

        while (elapsedTime < lightColorFadeSpeed)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / lightColorFadeSpeed);

            playerLight.color = Color.Lerp(currentLightColor, targetColor, t);

            await Awaitable.NextFrameAsync();
        }

        currentLightColor = playerLight.color;
    }

    private async void Die()
    {
        playerController.PlayerInput.CharacterControls.Disable();
        rotationController.enabled = false;
        isDead = true;
        animator.SetTrigger("Die");
        playerMovement.SetCurrentMovement(Vector3.zero);
        //playerMovement.enabled = false;
        //playerMovement.canWalk = false;
        // attackController.enabled = false;
        // defendController.enabled = false;
        // dodgeController.enabled = false;

        lightTransform.SetParent(this.transform.parent);

        await Awaitable.WaitForSecondsAsync(2f);

        await blackScreen.DeathScreen(isDeathProtected);

        if (!isDeathProtected)
        {
            lives--;
        }
        else
        {
            itemsController.RemoveItemById("04");
        }

        if (lives > 0)
        {
            characterController.enabled = false;

            await Awaitable.WaitForSecondsAsync(0.1f);

            transform.position = spawnPoint;

            await Awaitable.WaitForSecondsAsync(0.1f);

            characterController.enabled = true;

            livesCounter.text = Utils.ToRoman(lives);

            Revive();

            await Awaitable.WaitForSecondsAsync(1f);



            GracePeriod(gracePeriod);

            await blackScreen.FadeOut(blackScreen.FadeDuration);

            playerController.PlayerInput.CharacterControls.Enable();
            rotationController.enabled = true;
            // attackController.enabled = true;
            // defendController.enabled = true;
            // dodgeController.enabled = true;
        }
        else
        {
            await Awaitable.WaitForSecondsAsync(2f);
            ReturnToMainMenu();
        }
    }

    void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
        animator.SetTrigger("Revive");
        //playerMovement.enabled = true;
        //playerMovement.canWalk = true;
        // rotationController.enabled = true;
        // attackController.enabled = true;
        // defendController.enabled = true;
        // dodgeController.enabled = true;

        playerLight.color = defaultLightColor;
        currentLightColor = playerLight.color;

        lightTransform.SetParent(this.transform);
        lightTransform.position = lightPosition;
    }

    async void GracePeriod(float gracePeriod)
    {
        isInvincible = true;

        await Awaitable.WaitForSecondsAsync(gracePeriod);

        isInvincible = false;
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //CROWD CONTROL STUFF
    public bool HasCC() //add CCs here to check globally for impairing CCs in other components
    {
        if (isStaggered || isStunned)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //UNFINISHED
    // private void KnockBack(int healthDamage, int poiseDamage, float knockbackForce, float knockbackDuration, Transform source)
    // {
    //     if (!isDead)
    //     {
    //         currentHealth -= healthDamage;
    //         currentPoise -= poiseDamage;
    //         hitSound.Play();
    //         bloodSplatter.Play();
    //         if (currentHealth <= 0)
    //         {
    //             Die();
    //         }
    //         else if (!attackController.isAttacking)
    //         {
    //             animator.SetInteger("Knocked", 1);

    //             // Calculate the knockback direction
    //             // Vector3 knockbackDirection = (this.transform.position - source.position).normalized;
    //             // knockbackDirection.y = 0;
    //             // knockbackDirection.Normalize();

    //             // this.rb.velocity = Vector3.zero; // Reset the velocity
    //             // this.rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

    //             // StartCoroutine(WaitForKnockback(knockbackDuration));
    //         }
    //     }
    // }

    public void Stun(float stunDuration)
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");

        playerMovement.footStepsSound.enabled = false;
        playerMovement.SetCurrentMovement(Vector3.zero);
        playerMovement.canWalk = false;
        isStunned = true;

        animator.SetInteger("Stun", 1);
        PlayEffect(stunEffectName);
        StartCoroutine(StunTime(stunDuration * tenacity));
    }

    public void OnStunAnimation()
    {
        animator.SetInteger("Stun", 2);
    }

    private IEnumerator StunTime(float stunDuration)
    {
        yield return new WaitForSeconds(stunDuration);

        playerMovement.canWalk = true;
        isStunned = false;
        //rotationController.enabled = true;
        //attackController.canAttack = true;

        animator.SetInteger("Stun", 0);
        ClearEffect(stunEffectName);
        gameObject.layer = originalLayer;
    }

    public void Slow(float slowDuration, float slowStrength)
    {
        //Debug.Log("slowDuration: " + slowDuration + "/slowStrength: " + slowStrength);
        if (slowCoroutine != null)
        {
            slowRemainingTime = slowDuration * tenacity;
            newSlowStrength = slowStrength;
        }
        else
        {
            PlayEffect(slowEffectName);

            slowRemainingTime = slowDuration * tenacity;
            newSlowStrength = slowStrength;
            slowCoroutine = StartCoroutine(SlowTime());
        }
    }

    private IEnumerator SlowTime()
    {
        float currentSlowStrength = newSlowStrength;

        playerMovement.SetMoveSpeed(playerMovement.MoveSpeed / currentSlowStrength);
        playerMovement.footStepsSound.pitch /= currentSlowStrength;
        animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") / currentSlowStrength);
        //dodgeController.dodgePoiseCost = (int)(dodgeController.dodgePoiseCost * 5);

        while (slowRemainingTime > 0f)
        {
            if (newSlowStrength > currentSlowStrength)
            {
                playerMovement.SetMoveSpeed(playerMovement.MoveSpeed * currentSlowStrength);
                playerMovement.footStepsSound.pitch *= currentSlowStrength;
                animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") * currentSlowStrength);
                //dodgeController.dodgePoiseCost = (int)(dodgeController.dodgePoiseCost / currentSlowStrength);

                currentSlowStrength = newSlowStrength;

                playerMovement.SetMoveSpeed(playerMovement.MoveSpeed / currentSlowStrength);
                playerMovement.footStepsSound.pitch /= currentSlowStrength;
                animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") / currentSlowStrength);
                //dodgeController.dodgePoiseCost = (int)(dodgeController.dodgePoiseCost * currentSlowStrength);
            }

            slowRemainingTime -= Time.deltaTime;
            yield return null;
        }

        playerMovement.SetMoveSpeed(playerMovement.MoveSpeed * currentSlowStrength);
        playerMovement.footStepsSound.pitch *= currentSlowStrength;
        animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") * currentSlowStrength);
        //dodgeController.dodgePoiseCost = (int)(dodgeController.dodgePoiseCost / 5);

        ClearEffect(slowEffectName);

        slowCoroutine = null;
    }

    public void Blind(float blindDuration)
    {
        if (blindCoroutine != null)
        {
            blindRemainingTime = blindDuration * tenacity;
        }
        else
        {
            blindRemainingTime = blindDuration * tenacity;
            blindCoroutine = StartCoroutine(BlindTime());
        }
    }

    private IEnumerator BlindTime()
    {
        playerLight.spotAngle -= 100;

        while (blindRemainingTime > 0f)
        {
            blindRemainingTime -= Time.deltaTime;
            yield return null;
        }

        playerLight.spotAngle += 100;

        blindCoroutine = null;
    }

    public void PlayEffect(string effectName)
    {
        string address = "Assets/Particles/CC effects/" + effectName + ".prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject effectInstance = Instantiate(handle.Result, effectLocation.position, effectLocation.rotation);
                    effectInstance.transform.SetParent(effectLocation);
                }
                else
                {
                    Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };
    }

    public void ClearEffect(string effectName)
    {
        foreach (Transform child in effectLocation.transform)
        {
            if (child.name.StartsWith(effectName))
            {
                child.GetComponent<ParticleSystem>().Stop();
                StartCoroutine(DelayedDestroy(child, 1f));
            }
        }
    }

    //this is to prevent cc effects to disappear imediately, instead it lets it complete the particle lifecycle
    private IEnumerator DelayedDestroy(Transform objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject.Destroy(objectToDestroy.gameObject);
    }

    //animations stuff
    private IEnumerator WaitForKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetInteger("Knocked", 2);
    }

    public void OnKnockedAnimation()
    {
        playerMovement.enabled = false;
        animator.SetInteger("Knocked", 0);
    }

    public void OnGetUpAnimation()
    {
        animator.SetInteger("Knocked", 0);
    }

    public void OnGetUpAnimationEnd()
    {
        playerMovement.enabled = true;
    }

    private async void StaggerAsync(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }

        isStaggered = false;
        animator.SetFloat("StaggerMultiplier", 1.0f);
        playerMovement.canWalk = true;
    }

    public void OnHitAnimation()
    {
        animator.SetInteger("Hit", 0);
        //playerMovement.enabled = false;

        AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.IsName("Hit"))
        {
            playerMovement.canWalk = false;
            isStaggered = true;
            //Debug.Log("hit coroutine...");
            StaggerAsync(stateInfo.length);
        }
    }

    public void OnHitAnimationEnd()
    {
        //playerMovement.enabled = true;
        // playerMovement.canWalk = true;
        // isStaggered = false;
        // animator.SetFloat("StaggerMultiplier", 1.0f);
    }

}
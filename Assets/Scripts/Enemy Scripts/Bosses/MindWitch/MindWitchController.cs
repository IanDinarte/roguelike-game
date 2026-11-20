using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MindWitchController : MonoBehaviour
{
    private Animator animator;
    private MindWitchHitController hitController;
    public Transform player;
    private HitController playerHitController;

    private LineRenderer spellLine;
    private Vector3 lineBegin;
    private Vector3 lineEnd;

    public GameObject magicBarrier;
    public GameObject magicBarrier2;

    private float fightRange = 10f;
    private float stopFightRange = 35f;
    [SerializeField] private bool isFighting = false;

    public Transform[] teleportLocations = new Transform[6];
    private Vector3 currentLocation;

    private bool isCasting = false;
    private bool isTeleporting = false;
    private float distanceToFlee = 3f;
    private float teleportDelay = 1f;

    private float spellRange = 35f;
    private float channelDuration = 3f;
    private int spellDamage = 30;
    private float spellCooldown = 2f;
    private bool canCast = true;
    private GameObject mindDecayEffect;

    private float spawnCooldown = 60f;
    private bool canSpawn = false;

    //rotation variables
    public float rotationSpeed = 30.0f;
    public float rotationThreshold = 5.0f;
    private bool canRotate = true;

    public bool IsCasting => isCasting;
    public void SetIsCasting(bool result) => isCasting = result;
    public void SetCanRotate(bool state) => canRotate = state;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        hitController = this.GetComponent<MindWitchHitController>();
        spellLine = this.GetComponent<LineRenderer>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerHitController = player.gameObject.GetComponent<HitController>();
    }

    private void Start()
    {
        spellLine.enabled = false;
    }

    private void Update()
    {
        if (player != null)
        {

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= fightRange && !hitController.IsDead && !isFighting)
            {
                hitController.SetIsInvincible(false);
                isFighting = true;
                magicBarrier.SetActive(true);
                magicBarrier2.SetActive(true);
            }
            else if (distanceToPlayer >= stopFightRange || hitController.IsDead)
            {
                hitController.SetIsInvincible(true);
                isFighting = false;
                magicBarrier.SetActive(false);
                magicBarrier2.SetActive(false);
                canSpawn = false;
                hitController.healthBar.SetActive(false);
            }

            if (isFighting)
            {
                RotateTowardsPlayer();

                if (spellLine.enabled)
                {
                    lineEnd = new Vector3(player.position.x, player.position.y + 1.5f, player.position.z);
                    lineBegin = new Vector3(this.transform.position.x, this.transform.position.y + 3f, this.transform.position.z);
                    spellLine.SetPosition(0, lineBegin);
                    spellLine.SetPosition(1, lineEnd);
                }

                if (!hitController.healthBar.activeSelf)
                {
                    StartCoroutine(SpawnCooldown(spawnCooldown));

                    hitController.healthBar.SetActive(true);
                    hitController.healthBar.GetComponent<BossHealthBar>().Initialize();
                }

                if (distanceToPlayer <= distanceToFlee
                    && !isCasting
                    && !isTeleporting
                    && !hitController.IsStaggered)
                {
                    StartCoroutine(TeleportDelay(teleportDelay));
                }
                else if (canSpawn
                    && !isCasting
                    && !isTeleporting
                    && !hitController.IsStaggered)
                {
                    //Debug.Log("imma try spawning");
                    SpawnLackeys();
                }
                else if (distanceToPlayer <= spellRange
                    && canCast
                    && !isCasting
                    && !isTeleporting
                    && !hitController.IsStaggered)
                {
                    CastSpell();
                }
            }
        }
    }

    private IEnumerator TeleportDelay(float delay)
    {
        float elapsedTime = 0f;

        isTeleporting = true;

        while (elapsedTime < delay && isTeleporting)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isTeleporting)
        {
            animator.SetInteger("Teleport", 1);
        }
    }

    private Vector3 GetTeleportLocation(Vector3 currentLocation)
    {
        int index;
        do
        {
            index = Random.Range(0, teleportLocations.Length);
        } while (teleportLocations[index].position == currentLocation);

        return teleportLocations[index].position;
    }

    private IEnumerator Channeling(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration && isCasting)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isCasting)
        {
            playerHitController.TakeDamage(spellDamage, (int)(spellDamage / 1.5f), false, true);
            isCasting = false;
            spellLine.enabled = false;
        }

        Destroy(mindDecayEffect);
        StartCoroutine(SpellCooldown(spellCooldown));
    }

    public void Interrupt()
    {
        spellLine.enabled = false;
        isCasting = false;
        isTeleporting = false;
    }

    private IEnumerator SpellCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        canCast = true;
    }

    private void CastSpell()
    {
        spellLine.enabled = true;

        isCasting = true;
        canCast = false;
        animator.SetInteger("Casting", 1);

        string address = "Assets/Particles/Skill effects/Mind_Decay.prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        mindDecayEffect = Instantiate(handle.Result, player.position, player.rotation);
                        mindDecayEffect.transform.SetParent(player);
                        mindDecayEffect.GetComponent<ParticleSystem>().Play();
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab with address: {address}");
                    }
                };

        StartCoroutine(Channeling(channelDuration));
    }

    private IEnumerator SpawnCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        //Debug.Log("I can spawn");
        canSpawn = true;
    }

    private void SpawnLackeys()
    {
        int[] index = new int[2];
        do
        {
            index[0] = Random.Range(0, teleportLocations.Length);
            index[1] = Random.Range(0, teleportLocations.Length);
        } while (teleportLocations[index[0]].position != teleportLocations[index[1]].position
        && teleportLocations[index[0]].position != currentLocation
        && teleportLocations[index[1]].position != currentLocation);

        string address = "Assets/Prefabs/Entities/Enemies/Spectre.prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject specter1 = Instantiate(handle.Result, teleportLocations[index[0]].position, teleportLocations[index[0]].rotation);
                        GameObject specter2 = Instantiate(handle.Result, teleportLocations[index[1]].position, teleportLocations[index[1]].rotation);

                        specter1.GetComponent<EnemyController>().DamageAggro();
                        specter2.GetComponent<EnemyController>().DamageAggro();
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab with address: {address}");
                    }
                };

        canSpawn = false;
        StartCoroutine(SpawnCooldown(spawnCooldown));
    }

    protected void RotateTowardsPlayer()
    {
        if (canRotate)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void OnInitialTeleportAnimation()
    {
        //isTeleporting = true;
        hitController.SetIsInvincible(true);
        animator.SetInteger("Teleport", 0);
    }

    public void OnCastingAnimation()
    {
        animator.SetInteger("Casting", 0);
    }

    public void OnInitialTeleportAnimtionEnd()
    {
        this.transform.position = GetTeleportLocation(currentLocation);
    }

    public void OnFinalTeleportAnimationEnd()
    {
        currentLocation = this.transform.position;
        hitController.SetIsInvincible(false);
        isTeleporting = false;
    }
}
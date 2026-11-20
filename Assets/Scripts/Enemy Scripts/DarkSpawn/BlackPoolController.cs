using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BlackPoolController : MonoBehaviour
{
    private Transform player;
    public Transform spawnLocation;
    public AudioSource spawnSoundEffect;
    public ParticleSystem spawnParticleEffect;

    private float spawnRange = 5f;
    private float spawnCooldown = 10f;
    private bool canSpawn = true;

    public int spawnLimit = 5;
    private int spawnCounter = 0;

    public float slowDuration = 1f;
    public float slowStrength = 1.5f;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (canSpawn && distanceToPlayer <= spawnRange && spawnCounter < spawnLimit)
            {
                Spawn();
            }
        }
    }

    private IEnumerator SpawnCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        canSpawn = true;
    }

    private void Spawn()
    {
        canSpawn = false;

        string address = "Assets/Prefabs/Entities/Enemies/DarkSpawn.prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Instantiate(handle.Result, spawnLocation.position, spawnLocation.rotation);
                        spawnCounter++;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab with address: {address}");
                    }
                };

        StartCoroutine(SpawnCooldown(spawnCooldown));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("guiga");
            HitController player = other.GetComponent<HitController>();
            if (player != null)
            {
                player.Slow(slowDuration, slowStrength);
            }
        }
    }
}

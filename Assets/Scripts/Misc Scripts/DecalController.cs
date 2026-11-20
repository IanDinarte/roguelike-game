using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

public class DecalController : MonoBehaviour
{
    IObjectPool<DecalProjector> decalPool;

    public ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents;
    public string dpAddress = "Assets/Prefabs/Decals/BloodDecal.prefab";

    private GameObject decalPrefab;

    public float fadeDuration = 10f;

    private void Awake()
    {
        particleSystem = this.GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private async void Start()
    {
        decalPrefab = await Addressables.LoadAssetAsync<GameObject>(dpAddress).Task;

        if (decalPrefab == null)
        {
            Debug.LogError($"Failed to load prefab with address: {dpAddress}");
            return;
        }

        decalPool = new ObjectPool<DecalProjector>(
        createFunc: () =>
        {
            GameObject go = Instantiate(decalPrefab);

            DecalProjector dp = go.GetComponent<DecalProjector>();
            return dp;
        },
        actionOnGet: dp => dp.gameObject.SetActive(true),
        actionOnRelease: dp => dp.gameObject.SetActive(false),
        actionOnDestroy: dp => Destroy(dp.gameObject),
        collectionCheck: false,
        defaultCapacity: 10,
        maxSize: 20
        );
    }

    private IEnumerator FadeAndRelease(DecalProjector projector, float duration)
    {
        float time = 0f;
        float initialFade = projector.fadeFactor;

        while (time < duration)
        {
            if (projector == null) yield break;

            time += Time.deltaTime;
            float t = time / duration;
            projector.fadeFactor = Mathf.Lerp(initialFade, 0f, t);
            yield return null;
        }

        if (projector != null)
        {
            projector.fadeFactor = initialFade;
            decalPool.Release(projector);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
        {
            DecalProjector projector = decalPool.Get();

            projector.transform.position = collisionEvents[i].intersection + collisionEvents[i].normal * 0.02f;

            Quaternion normalRotation = Quaternion.LookRotation(-collisionEvents[i].normal, Vector3.up);
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            projector.transform.rotation = normalRotation * randomRotation;

            float randomSize = Random.Range(0.25f, 1f);

            projector.size = new Vector3(randomSize, randomSize, 1f);

            if (fadeDuration > 0)
            {
                StartCoroutine(FadeAndRelease(projector, fadeDuration));
            }
            i++;
        }
    }
}

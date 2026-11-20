using System.Collections;
using System.Threading;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private Light fireLight;

    [SerializeField] private int minIntensity = 11;
    [SerializeField] private int maxIntensity = 14;
    [SerializeField] private float flickerSpeed = 0.075f;

    private CancellationTokenSource cts;

    private void Awake()
    {
        if (fireLight == null)
            fireLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        FlickerAsync(cts.Token);
    }

    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async void FlickerAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && fireLight != null)
            {
                fireLight.intensity = Random.Range(minIntensity, maxIntensity);
                await Awaitable.WaitForSecondsAsync(flickerSpeed, token);
            }
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Flicker cancelled cleanly.");
        }
    }
}

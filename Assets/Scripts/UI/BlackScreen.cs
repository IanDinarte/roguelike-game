using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class BlackScreen : MonoBehaviour
{
    private HitController playerHitController;

    private GameObject oldLifeObject;
    private GameObject newLifeObject;
    private TextMeshProUGUI oldLife;
    private TextMeshProUGUI newLife;
    private RectTransform oldLifePos;
    private RectTransform newLifePos;
    [SerializeField] private Vector3 oldLifeDefaultPosition;
    [SerializeField] private Vector3 newLifeDefaultPosition;

    private CanvasGroup canvasGroup;

    private GameObject blackBorder1;
    private GameObject blackBorder2;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float scrollSpeed = 10f;
    [SerializeField] private float colorFadeDuration = 1.5f;
    [SerializeField] private Color32 protectColor = new Color32(0, 145, 255, 255);


    // private bool fading = false;
    // private bool scrolling = false;

    public float FadeDuration => fadeDuration;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHitController = player.GetComponent<HitController>();
        }

        oldLifeObject = this.transform.Find("OldLife").gameObject;
        newLifeObject = this.transform.Find("NewLife").gameObject;

        oldLifePos = oldLifeObject.GetComponent<RectTransform>();
        newLifePos = newLifeObject.GetComponent<RectTransform>();

        // oldLifeDefaultPosition = oldLifeObject.transform.position;
        // newLifeDefaultPosition = newLifeObject.transform.position;
        
        oldLifeDefaultPosition = oldLifePos.anchoredPosition;
        newLifeDefaultPosition = newLifePos.anchoredPosition;

        if (oldLifeObject && newLifeObject)
        {
            oldLife = oldLifeObject.GetComponent<TextMeshProUGUI>();
            newLife = newLifeObject.GetComponent<TextMeshProUGUI>();
        }

        blackBorder1 = this.transform.Find("BlackBorder1").gameObject;
        blackBorder2 = this.transform.Find("BlackBorder2").gameObject;

        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    private async void Start()
    {
        EnableDeathCounter(false);

        await FadeOut(fadeDuration);
    }

    public void EnableDeathCounter(bool decision)
    {
        oldLifeObject.SetActive(decision);
        newLifeObject.SetActive(decision);
        blackBorder1.SetActive(decision);
        blackBorder2.SetActive(decision);

    }

    public async Task FadeIn(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha += 1 / duration * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }
    }

    public async Task FadeOut(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {

            canvasGroup.alpha -= 1 / duration * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }

        EnableDeathCounter(false);
    }

    public async Task DeathScreen(bool isDeathProtected)
    {
        oldLifeObject.SetActive(true);

        oldLife.color = Color.white;

        oldLife.text = Utils.ToRoman(playerHitController.Lives);

        if (playerHitController.Lives == 1)
        {
            newLife.text = "<color=red>ROT</color>";
        }
        else
        {
            newLife.text = Utils.ToRoman(playerHitController.Lives - 1);
        }

        await FadeIn(fadeDuration);

        blackBorder1.SetActive(true);
        blackBorder2.SetActive(true);
        newLifeObject.SetActive(true);

        if (isDeathProtected)
        {
            await FadeTextColor(oldLife, colorFadeDuration, protectColor);
        }
        else
        {
            await ScrollCounter(scrollSpeed);
        }

        blackBorder1.SetActive(false);
        blackBorder2.SetActive(false);
        newLifeObject.SetActive(false);
        //await FadeOut(fadeDuration);
    }

    private async Awaitable FadeTextColor(TextMeshProUGUI textObject, float fadeDuration, Color endColor)
    {
        float elapsedTime = 0f;
        Color startColor = textObject.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            textObject.color = Color.Lerp(startColor, endColor, t);

            await Awaitable.NextFrameAsync();
        }

        textObject.color = endColor;
    }

    private async Task ScrollCounter(float speed)
    {
        float idealHeight = oldLifePos.anchoredPosition.y;
        Debug.Log(idealHeight);

        Vector3 pos1 = oldLifePos.anchoredPosition;
        Vector3 pos2 = newLifePos.anchoredPosition;

        while (newLifePos.anchoredPosition.y > idealHeight)
        {
            oldLifePos.anchoredPosition = pos1;
            newLifePos.anchoredPosition = pos2;

            pos1.y -= speed * Time.deltaTime;
            pos2.y -= speed * Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }

        oldLife.text = newLife.text;
        oldLifePos.anchoredPosition = oldLifeDefaultPosition;
        newLifePos.anchoredPosition = newLifeDefaultPosition;

        //await Awaitable.WaitForSecondsAsync(1f);
    }
}
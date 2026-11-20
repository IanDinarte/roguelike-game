using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    [SerializeField] private RectTransform creditsPos;
    [SerializeField] private GameObject gameTitle;
    [SerializeField] private RectTransform gameTitlePos;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private GameObject redEyes;
    [SerializeField] private BlackScreen blackScreen;
    [SerializeField] private AudioSource music;

    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private float fadeOutDuration = 2f;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
        creditsPos = credits.GetComponent<RectTransform>();
        gameTitlePos = gameTitle.GetComponent<RectTransform>();
    }

    private async void Start()
    {
        redEyes.SetActive(false);

        await ScrollCredits();

        redEyes.SetActive(true);

        await Awaitable.WaitForSecondsAsync(2f);

        while (music.isPlaying)
        {
            await Awaitable.NextFrameAsync();
        }

        await FadeOutTitle();

        await Awaitable.WaitForSecondsAsync(5f);

        await blackScreen.FadeIn(blackScreen.FadeDuration);

        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    private async Awaitable ScrollCredits()
    {
        Vector3 pos = creditsPos.anchoredPosition;
        Vector3 titlePos = gameTitlePos.anchoredPosition;

        while (gameTitle.transform.position.y < 570f)
        {
            pos.y += scrollSpeed * Time.deltaTime;
            titlePos.y += scrollSpeed * Time.deltaTime;
            creditsPos.anchoredPosition = pos;
            //gameTitle.transform.position = titlePos;

            await Awaitable.NextFrameAsync();
        }
    }

    public async Awaitable FadeOutTitle()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeOutDuration)
        {

            titleCanvasGroup.alpha -= 1 / fadeOutDuration * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }
    }

    public void InstagramLink()
    {
        Application.OpenURL("https://www.instagram.com/guandark/");
    }

    public void SoundcloudLink()
    {
        Application.OpenURL("https://soundcloud.com/palliative5");

    }
}
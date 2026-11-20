using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PassageController : MonoBehaviour
{
    [SerializeField] private Level level;

    private GameObject loadingBar;
    private Slider loadingSlider;
    private GameObject pressAnyKey;
    private GameObject loadingScreen;

    private void Start()
    {
        if (level.LoadingScreen != null)
        {
            loadingBar = level.LoadingScreen.transform.Find("LoadingBar").gameObject;
            loadingSlider = loadingBar.GetComponent<Slider>();
            pressAnyKey = level.LoadingScreen.transform.Find("PressAnyKey").gameObject;
            level.LoadingScreen.transform.Find("LevelName").gameObject.GetComponent<TextMeshProUGUI>().text = level.LevelName;
            loadingScreen = level.LoadingScreen;
        }
    }

    public void LoadLevel()
    {
        if (level.LoadingScreen == null)
        {
            LoadNoLoadingScreen();
        }
        else
        {
            LoadLoadingScreen();
        }
    }

    private void LoadNoLoadingScreen()
    {
        SceneManager.LoadScene(level.LevelIndex);
    }

    private async void LoadLoadingScreen()
    {
        loadingScreen = Instantiate(loadingScreen);

        DontDestroyOnLoad(loadingScreen);

        await Awaitable.NextFrameAsync();

        await LoadLevelAsync(level.LevelIndex);

        LoadingScreen();
    }

    async void LoadingScreen()
    {
        Time.timeScale = 0;

        loadingBar.SetActive(false);
        pressAnyKey.SetActive(true);

        await Utils.AnyKeyPress();

        Destroy(loadingScreen);

        Time.timeScale = 1;
    }

    async Awaitable LoadLevelAsync(int levelIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelIndex);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            await Awaitable.NextFrameAsync();
        }
    }
}
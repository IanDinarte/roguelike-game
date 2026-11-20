using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuText;
    [SerializeField] private GameObject loadingBar;
    private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI versionFooter;
    [SerializeField] private GameObject pressAnyKey;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();

        //playerInput.CharacterControls.Disable();
    }

    private void Start()
    {
        Time.timeScale = 1;

        loadingSlider = loadingBar.GetComponent<Slider>();
        versionFooter.text = "Version " + Application.version;
    }

    public async void NewGame()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        DontDestroyOnLoad(loadingScreen);

        await Awaitable.NextFrameAsync();

        await LoadLevelAsync(SceneManager.GetActiveScene().buildIndex + 1);

        LoadingScreen();
    }

    public async void Continue()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        DontDestroyOnLoad(loadingScreen);

        await Awaitable.NextFrameAsync();

        await LoadLevelAsync(1); //take stored level index in gamemanager maybe

        LoadingScreen();
    }

    public void ShowTutorial()
    {
        mainMenuText.SetActive(false);
        tutorialScreen.SetActive(true);
    }

    public void ExitTutorial()
    {
        mainMenuText.SetActive(true);
        tutorialScreen.SetActive(false);
    }

    public void Settings()
    {
        mainMenuText.SetActive(false);
        settingsScreen.SetActive(true);
    }

    public void ExitSettings()
    {
        mainMenuText.SetActive(true);
        settingsScreen.SetActive(false);
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void InstagramLink()
    {
        Application.OpenURL("https://www.instagram.com/guandark/");
    }

    public void SoundcloudLink()
    {
        Application.OpenURL("https://soundcloud.com/palliative5");

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
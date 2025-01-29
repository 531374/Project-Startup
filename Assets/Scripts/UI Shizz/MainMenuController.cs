using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenuController : MonoBehaviour
{

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle FullScreenToggle;

    [Header ("Main Menu Functions")] 
    public string newGameLevel;
    public string loadGameLevel;

    public void NewGameScene()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(loadGameLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    [Header("Audio Settings Functions")]

    [SerializeField] private TMP_Text volumeValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        { AudioListener.volume = defaultVolume;
        volumeSlider.value = defaultVolume;
            volumeValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            SensValue.text = defaultSens.ToString("0.0");
            SensSlider.value = defaultSens;
            mainSens = defaultSens;
            invertY.isOn = false;
            GameplayApply();
        }

        if (MenuType == "Graphics")
        {
            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);
            FullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;

            fpsToggle.isOn = false;
            FpsDisplay.gameObject.SetActive(false);
            PlayerPrefs.SetInt("displayFPS", 0);

            GraphicsApply();
        }

    }


    [Header("Gameplay Settings Functions")]
    [SerializeField] private TMP_Text SensValue = null;
    [SerializeField] private Slider SensSlider = null;
    [SerializeField] private float defaultSens = 1;
    public float mainSens = 1;

    [SerializeField] private Toggle invertY = null;

    public void SetSens(float sensitivity)
    {
        //mainSens = Mathf.RoundToInt(sensitivity);
        mainSens = sensitivity;
        SensValue.text = sensitivity.ToString("0.0");
    }

    public void GameplayApply()
    {
        if (invertY.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else { PlayerPrefs.SetInt("masterInvertY", 0); }

        PlayerPrefs.SetFloat("masterSens", mainSens);
    }


    [Header("Graphics Settings Functions")]

    private int _qualityLevel;
    private bool _isFullscreen;

    [SerializeField] private Toggle fpsToggle;

    public void SetFullscreen(bool isFullscreen)
    {
        _isFullscreen = isFullscreen;       
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1 : 0));
        Screen.fullScreen = _isFullscreen;

        PlayerPrefs.SetInt("displayFPS", fpsToggle.isOn ? 1 : 0);
        FpsDisplay.gameObject.SetActive(fpsToggle.isOn);
    }

    [Header("Graphics Settings Functions")]

    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public TextMeshProUGUI FpsDisplay;
    private float pollingTime = 1.5f;
    private float time;
    private int frameCount;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Load FPS toggle state
        bool isFpsDisplayed = PlayerPrefs.GetInt("displayFPS", 0) == 1;
        fpsToggle.isOn = isFpsDisplayed;
        FpsDisplay.gameObject.SetActive(isFpsDisplayed);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //public void ToggleFPSDisplay(bool isDisplayed)
    //{
    //    FpsDisplay.gameObject.SetActive(isDisplayed);
    //    PlayerPrefs.SetInt("displayFPS", isDisplayed ? 1 : 0);
    //}

    private void Update()
    {
        time += Time.deltaTime;
        frameCount++;
        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            FpsDisplay.text = frameRate.ToString() + " FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }


}

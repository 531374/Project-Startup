using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    public static LoadPrefs instance;

    [SerializeField] private bool canUse=false;
    [SerializeField] private MainMenuController menuController;


    //Audio
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;

    //Graphics
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle FullScreenToggle;
    [SerializeField] private Toggle fpsToggle;

    //Gameplay
    [SerializeField] private Toggle invertY = null;
    [SerializeField] private TMP_Text SensValue = null;
    [SerializeField] private Slider SensSlider = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy (gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad (gameObject);

        InitializePrefs();
    }

    void InitializePrefs ()     
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
            else
            {
                menuController.ResetButton("Audio");
            }


            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");
                qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }

            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                if (localFullscreen == 1)
                {
                    Screen.fullScreen = true;
                    FullScreenToggle.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    FullScreenToggle.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("masterSens"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSens");

                SensValue.text = localSensitivity.ToString("0.0");
                SensSlider.value = localSensitivity;
                menuController.mainSens = Mathf.RoundToInt(localSensitivity);
            }

            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                if (PlayerPrefs.GetInt("masterInvertY") == 1)
                {
                    invertY.isOn = true;
                }

                else
                {
                    invertY.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("displayFPS"))
            {
                if (PlayerPrefs.GetInt("displayFPS") == 1)
                {
                    fpsToggle.isOn = true;
                }

                else
                {
                    fpsToggle.isOn = false;
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializePrefs();
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenuController : MonoBehaviour
{

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

    public void VolumeReset(string MenuType)
    {
        if (MenuType == "Audio")
        { AudioListener.volume = defaultVolume;
        volumeSlider.value = defaultVolume;
            volumeValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            SensValue.text = defaultSens.ToString("0");
            SensSlider.value = defaultSens;
            mainSens = defaultSens;
            invertY.isOn = false;
            GameplayApply();
        }

    }


    [Header("Gameplay Settings Functions")]
    [SerializeField] private TMP_Text SensValue = null;
    [SerializeField] private Slider SensSlider = null;
    [SerializeField] private int defaultSens = 5;
    public int mainSens = 5;

    [SerializeField] private Toggle invertY = null;

    public void SetSens(float sensitivity)
    {
        mainSens = Mathf.RoundToInt(sensitivity);
        SensValue.text = sensitivity.ToString("0");
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
}

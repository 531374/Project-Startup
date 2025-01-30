using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BookManager : MonoBehaviour
{
    [HideInInspector] public static BookManager instance;

    [Header("References")]
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject journal;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject settings;
    [SerializeField] private AudioClip switchSound; // Reference to the sound clip
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private Button journalButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Sprite journalSelectedButton;
    [SerializeField] private Sprite mapSelectedButton;
    [SerializeField] private Sprite inventorySelectedButton;
    [SerializeField] private Sprite settingsSelectedButton;
    [SerializeField] private Sprite journalImage;
    [SerializeField] private Sprite mapImage;
    [SerializeField] private Sprite inventoryImage;
    [SerializeField] private Sprite settingsImage;
    public bool isBookOpened;
    [HideInInspector] public bool isPaused = false;

    private void Awake ()
    {
        if (instance == null) instance = this;
    }


    private void Update ()
    {
        OpenBook ();

        if (book.activeSelf && !isBookOpened) isBookOpened = true;
        else if (!book.activeSelf && isBookOpened) isBookOpened = false;
    }

    private void OpenBook()
    {

        if (Input.GetKeyDown (KeyCode.J))
        {
            if (!journal.activeSelf)
            {
                setJournal();
            }
            else
            {
                ResumeGame();
                DisableJournal();
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (!miniMap.activeSelf)
            {
                setMap();
            }
            else
            {
                ResumeGame();
                DisableMap();
            }
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventory.activeSelf)
            {
                setInventory();
            }
            else
            {
                ResumeGame();
                DisableInventory();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settings.activeSelf)
            {
                setSettings();
            }
            else
            {
                ResumeGame();
                DisableSettings();
            }
        }
    }


    public void setJournal()
    {
        PlaySound();
        EnableJournal();
        DisableInventory();
        DisableMap();
        DisableSettings();
        PauseGame();

        EventSystem.current.SetSelectedGameObject(null);
        journalButton.transform.GetChild(0).GetComponent<Image> ().sprite = journalSelectedButton;
    }

    public void setInventory()
    {
        PlaySound();
        EnableInventory();
        DisableJournal();
        DisableMap();
        DisableSettings();
        PauseGame();

        EventSystem.current.SetSelectedGameObject(null);
        inventoryButton.transform.GetChild(0).GetComponent<Image> ().sprite = inventorySelectedButton;
    }

    public void setMap()
    {
        PlaySound();
        EnableMap();
        DisableJournal();
        DisableInventory();
        DisableSettings();
        PauseGame();

        EventSystem.current.SetSelectedGameObject(null);
        mapButton.transform.GetChild(0).GetComponent<Image>().sprite = mapSelectedButton;
    }

    public void setSettings()
    {
        PlaySound();
        EnableSettings();
        DisableInventory();
        DisableJournal();
        DisableMap();
        PauseGame();

        EventSystem.current.SetSelectedGameObject(null);
        settingsButton.transform.GetChild(0).GetComponent<Image> ().sprite = settingsSelectedButton;
    }


    private void EnableJournal()
    {
        book.SetActive(true);
        journal.SetActive(true);
    }

    private void EnableInventory()
    {
        book.SetActive(true);
        inventory.SetActive(true);
    }

    private void EnableMap()
    {
        book.SetActive(true);
        miniMap.SetActive(true);
    }

    private void EnableSettings()
    {
        book.SetActive(true);
        settings.SetActive(true);
    }

        private void DisableJournal()
    {
        journal.SetActive(false);
        journalButton.image.sprite = journalImage;
    }

    private void DisableInventory()
    {
        inventory.SetActive(false);
        inventoryButton.image.sprite = inventoryImage;
    }

    private void DisableMap()
    {
        miniMap.SetActive(false);
        mapButton.image.sprite = mapImage;
    }

    private void DisableSettings()
    {
        settings.SetActive(false);
        settingsButton.image.sprite = settingsImage;
    }

    private void PlaySound()
    {
        if (audioSource != null && switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
    }
}

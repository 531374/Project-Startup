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

    private void OpenBook ()
    {

        if (Input.GetKeyDown (KeyCode.J))
        {
            if (book.activeSelf == false && journal.activeSelf == false) 
            {
                book.SetActive (true);
                setJournal ();
            }
            else if (book.activeSelf == true && journal.activeSelf == true) 
            {
                ResumeGame ();
                DisableJournal ();
                book.SetActive (false);
            }
           
        } else if (Input.GetKeyDown (KeyCode.M))
        {
            if (book.activeSelf == false && miniMap.activeSelf == false) 
            {
                book.SetActive (true);
                setMap ();
            }
            else if (book.activeSelf == true && miniMap.activeSelf == true) 
            {
                ResumeGame ();
                DisableMap ();
                book.SetActive (false);
            }
      
        }else if (Input.GetKeyDown (KeyCode.I)) 
        {
            if (book.activeSelf == false && inventory.activeSelf == false)
            { 
                book.SetActive (true);
                setInventory ();
            }
            else if (book.activeSelf == true && inventory.activeSelf == true) 
            {
                ResumeGame ();
                DissableInventory ();
                book.SetActive (false);
            }
         
        }
        else if (Input.GetKeyDown (KeyCode.Escape) && book.activeSelf == false)
        {
            book.SetActive (true);
            setSettings ();
        } else if (Input.GetKeyDown (KeyCode.Escape) && book.activeSelf == true)
        {
            ResumeGame ();
            DisableSettings ();
            book.SetActive (false);
        }
    }

    public void setJournal()
    {
        PlaySound();
        EnableJournal();
        DissableInventory ();
        DisableMap();
        DisableSettings();
        PauseGame ();
        if (!EventSystem.current.currentSelectedGameObject == journalButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            journalButton.image.sprite = journalSelectedButton;
        }
    }

    public void setInventory()
    {
        PlaySound();
        DisableJournal();
        EnableInvetory();
        DisableMap();
        DisableSettings();
        PauseGame ();
        if (!EventSystem.current.currentSelectedGameObject == inventoryButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            inventoryButton.image.sprite = inventorySelectedButton;
        }
    }
    public void setMap()
    {
        PlaySound();
        EnableMap();
        DisableJournal();
        DissableInventory ();
        DisableSettings();
        PauseGame ();
        if (!EventSystem.current.currentSelectedGameObject == mapButton)
        {   
            EventSystem.current.SetSelectedGameObject(null);
            mapButton.image.sprite = mapSelectedButton;
        }
    }

    public void setSettings()
    {
        PlaySound();
        EnableSettings();
        DissableInventory ();
        DisableJournal();
        DisableMap();
        PauseGame ();
        if (!EventSystem.current.currentSelectedGameObject == settingsButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            settingsButton.image.sprite = settingsSelectedButton;
        }

    }

    private void DisableJournal()
    {
        if (journal.activeSelf == true) journal.SetActive(false);
        journalButton.image.sprite = journalImage;
    }

    private void EnableJournal()
    {
        if (journal.activeSelf == false) journal.SetActive(true);
    }

    private void EnableInvetory ()
    {
        if (inventory.activeSelf == false) inventory.SetActive (true);
    }

    private void DissableInventory ()
    {
        if (inventory.activeSelf == true) inventory.SetActive (false);
        inventoryButton.image.sprite = inventoryImage;
    }

    private void DisableMap()
    {
        if (miniMap.activeSelf == true) miniMap.SetActive(false);
        mapButton.image.sprite = mapImage;
    }

    private void EnableMap()
    {
        if (miniMap.activeSelf == false) miniMap.SetActive(true);
    }

    private void DisableSettings()
    {
        if (settings.activeSelf == true) settings.SetActive(false);
        settingsButton.image.sprite = settingsImage;
    }

    private void EnableSettings()
    {
        if (settings.activeSelf == false) settings.SetActive(true);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    [Header("References")]
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject journal;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject settings;
    [SerializeField] private AudioClip switchSound; // Reference to the sound clip
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
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
    }

    public void setInventory()
    {
        PlaySound();
        DisableJournal();
        EnableInvetory();
        DisableMap();
        DisableSettings();
        PauseGame ();
    }
    public void setMap()
    {
        PlaySound();
        EnableMap();
        DisableJournal();
        DissableInventory ();
        DisableSettings();
        PauseGame ();
    }

    public void setSettings()
    {
        PlaySound();
        EnableSettings();
        DissableInventory ();
        DisableJournal();
        DisableMap();
        PauseGame ();
    }

    private void DisableJournal()
    {
        if (journal.activeSelf == true) journal.SetActive(false);
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
    }

    private void DisableMap()
    {
        if (miniMap.activeSelf == true) miniMap.SetActive(false);
    }

    private void EnableMap()
    {
        if (miniMap.activeSelf == false) miniMap.SetActive(true);
    }

    private void DisableSettings()
    {
        if (settings.activeSelf == true) settings.SetActive(false);
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

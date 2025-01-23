using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject journal;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject settings;
    [SerializeField] private AudioClip switchSound; // Reference to the sound clip
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource

    public static bool isPaused = false;


    private void Update ()
    {
        OpenBook ();
    }

    private void OpenBook ()
    {
        if (Input.GetKeyDown (KeyCode.J))
        {
            if (book.activeSelf == false && journal.activeSelf == false) book.SetActive (true);
            else if (book.activeSelf == true && journal.activeSelf == true) 
            {
                book.SetActive (false);
                ResumeGame ();
            }
            setJournal ();
        } else if (Input.GetKeyDown (KeyCode.M))
        {
            if (book.activeSelf == false && miniMap.activeSelf == false) book.SetActive (true);
            else if (book.activeSelf == true && miniMap.activeSelf == true) 
            {
                book.SetActive (false);
                ResumeGame ();
            }
            setMap ();
        } else if (Input.GetKeyDown (KeyCode.Escape) && book.activeSelf == false)
        {
            book.SetActive (true);
            setSettings ();
        } else if (Input.GetKeyDown (KeyCode.Escape) && book.activeSelf == true)
        {
            book.SetActive (false);
            ResumeGame ();
        }
    }

    public void setJournal()
    {
        PlaySound();
        EnableJournal();
        DisableMap();
        DisableSettings();
        PauseGame ();
    }

    public void setMap()
    {
        PlaySound();
        EnableMap();
        DisableJournal();
        DisableSettings();
        PauseGame ();
    }

    public void setSettings()
    {
        PlaySound();
        EnableSettings();
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
        if (!isPaused)
        {
            Time.timeScale = 0;
            isPaused = false;
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            Debug.Log ("asd");
            Time.timeScale = 1;
            isPaused = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipSceneSwitcher : InteractableObject
{
    [SerializeField] private float sceneLoadingRange;
    private Transform player;

    private void Start ()
    {
        player = GameObject.FindGameObjectWithTag ("Ship").transform;
        if (player == null) player = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    protected override void Interact()
    {
        base.Interact();

        if (interacted && SceneManager.GetActiveScene() == SceneManager.GetSceneByName ("Jasper Scene"))
        {
            SceneManager.LoadScene ("Ship");
            interacted = false;
        }
        else if (interacted && SceneManager.GetActiveScene() == SceneManager.GetSceneByName ("Ship"))
        {
            SceneManager.LoadScene ("Jasper Scene");
            interacted = false;
        }
    }
}

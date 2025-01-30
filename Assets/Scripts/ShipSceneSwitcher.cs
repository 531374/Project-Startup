using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipSceneSwitcher : InteractableObject
{
    [SerializeField] private float sceneLoadingRange;
    //private Transform player;

    private void Start ()
    {

    }

    protected override void Update()
    {
        base.Update();
        
        if (interacted && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Enemy Scene"))
        {
            SceneManager.LoadScene("Open World");
            interacted = false;
        }
        else if (interacted && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Open World"))
        {
            SceneManager.LoadScene("Enemy Scene");
            interacted = false;
        }
    }
    protected override void Interact()
    {
        base.Interact();
    }
}

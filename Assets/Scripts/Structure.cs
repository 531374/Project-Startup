using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Structure : InteractableObject
{
    [SerializeField] private Sprite icon; // Assign this in the Inspector
    public JournalEntry JournalEntry;
    private UnityEvent animationEventTrigger = new UnityEvent ();
    [SerializeField] private float detectionRange;
    private LayerMask playerLayer = 7;
    [SerializeField]private float height = 0f;
    private StructureAnimationManager animManager;
    private Transform doorPoint;
    [SerializeField] private GameObject interiorPrefab;
    private Transform sceneTransform;

    private void Start ()
    {
        animManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<StructureAnimationManager> ();
        this.gameObject.tag = "Structure";
        animationEventTrigger.AddListener (() => animManager.PlayAnimation ());
        height = transform.localScale.y;
    }

    protected override void Update ()
    {
        base.Update ();
        if (keyCap != null && keyCap.transform.position != transform.GetChild (1).transform.position) 
        {
            keyCap.transform.position = transform.GetChild (1).transform.position;
            keyCap.transform.localScale = new Vector3 (2f, 2f, 2f);
        }
        triggerAnimation ();
        interaction ();
    }

    private void interaction()
    {
        if (interacted)
        {
            if (interiorPrefab != null)
            {
                // Start scene loading asynchronously
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

                // Wait until the scene is loaded
                asyncLoad.completed += (operation) =>
                {
                    // After the scene is loaded, check for the object
                    Scene loadedScene = SceneManager.GetSceneByBuildIndex(1);

                    if (loadedScene.isLoaded)
                    {
                        // Search for the GameObject in the newly loaded scene
                        GameObject targetObject = GameObject.Find("Player Camera");

                        if (targetObject != null)
                        {
                            Debug.Log($"Found object: {targetObject.name}");
                            targetObject.transform.position = new Vector3 (0, 7, -15);
                        }
                        else
                        {
                            Debug.LogWarning("Target object not found in the loaded scene.");
                        }

                        // Optional: Instantiate the prefab in the loaded scene
                        GameObject interior = Instantiate(interiorPrefab);
                        SceneManager.MoveGameObjectToScene(interior, loadedScene);
                    }
                };
            }
            else
            {
                Debug.LogError("Interior prefab is not assigned!");
            }

            // Reset interaction state
            interacted = false;
        }
    }




    void triggerAnimation ()
    {
        Collider[] colliders = Physics.OverlapBox (this.transform.position + new Vector3 (0, height, 0), 
                                                  new Vector3 (detectionRange /2, detectionRange / 2, detectionRange / 2) ,Quaternion.identity, playerLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag ("Player") || collider.CompareTag ("Ship"))
            {
                animationEventTrigger.Invoke ();
            }
        }
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public TMP_Text GetEntryName ()
    {
        return JournalEntry.GetName();
    }

    public TMP_Text GetEntryText ()
    {
        return JournalEntry.GetEntryText ();
    }

    public JournalEntry GetJournalEntry ()
    {
        return JournalEntry;
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube (this.transform.position + new Vector3 (0, height / 2, 0), new Vector3 (detectionRange, detectionRange, detectionRange));
    }
}

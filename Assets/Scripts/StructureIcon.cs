using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class StructureIcon : MonoBehaviour
{
    [SerializeField] private Sprite icon; // Assign this in the Inspector
    public JournalEntry JournalEntry;
    private UnityEvent animationEventTrigger = new UnityEvent ();
    [SerializeField] private float detectionRange;
    private LayerMask playerLayer = 7;
    [SerializeField]private float height = 0f;
    private StructureAnimationManager animManager;

    private void Start ()
    {
        animManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<StructureAnimationManager> ();
        Debug.Log (animManager);
        this.gameObject.tag = "Structure";
        animationEventTrigger.AddListener (() => animManager.PlayAnimation ());
        height = transform.localScale.y;
    }

    void Update ()
    {
        triggerAnimation ();
    }

    void triggerAnimation ()
    {
        Collider[] colliders = Physics.OverlapBox (this.transform.position + new Vector3 (0, height, 0), 
                                                  new Vector3 (detectionRange /2, detectionRange / 2, detectionRange / 2) ,Quaternion.identity, playerLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag ("Player"))
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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntriesManager : MonoBehaviour
{
    [SerializeField] private GameObject entryTextContainer;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private TMP_Text textPref; // Prefab for TMP_Text
    [SerializeField] private Transform entries; // Parent transform for text entries

    private Structure currentlyOpenEntry; // Track currently open entry
    private float height;
    private float entryHeight;

    private Dictionary<GameObject, JournalEntry> entriesAlreadyAdded = new Dictionary<GameObject, JournalEntry>();

    RectTransform rectTransformParent;

    private List<Transform> children = new List<Transform> ();

    void Start()
    {

        rectTransformParent = entryTextContainer.GetComponent<RectTransform>();
        height = 0;
    }

    void Update()
    {
        if (entries.childCount > 0)
        {
            foreach (Transform child in entries)
            {
                if (children.Contains (child)) continue;

                RectTransform rectTransform = child.GetComponent<RectTransform>();
                if (rectTransform != null && rectTransform.rect.height > 0)
                {
                    height += rectTransform.rect.height;
                }

                children.Add (child);
            }
        }

        
        foreach (GameObject structure in mapManager.visitedStructures)
        {
            // Skip if we already have this structure in our entries
            if (entriesAlreadyAdded.ContainsKey(structure))
            {
                continue;
            }

            Structure structureIcon = structure.GetComponent<Structure>(); 
            entriesAlreadyAdded.Add(structure, structureIcon.GetJournalEntry());

            if (structureIcon == null) continue;

            // Create new entry only for structures we haven't processed yet
            TMP_Text newText = Instantiate(structureIcon.GetEntryName(), entries);

            Button button = newText.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => HandleEntryClick(structureIcon));
        }


        if (height != rectTransformParent.rect.height)
        {
            Vector2 sizeDelta = rectTransformParent.sizeDelta;
            sizeDelta.y = height;
            rectTransformParent.sizeDelta = sizeDelta;
        }

    }

    private void HandleEntryClick(Structure entry)
    {
        if (currentlyOpenEntry == entry)
        {
            // If clicking the same entry that's already open, close it
            ClearEntryText();
            currentlyOpenEntry = null;
        }
        else
        {
            // Open the new entry
            SetEntryText(entry);
            currentlyOpenEntry = entry;
        }
    }

    private void ClearEntryText()
    {
        if (entryTextContainer.transform.childCount > 0)
        {
            GameObject childToDestroy = entryTextContainer.transform.GetChild(0).gameObject;
            Destroy(childToDestroy);
        }
    }

    public void SetEntryText(Structure entry)
    {
        // First check if there are any children
        if (entryTextContainer.transform.childCount > 0)
        {
            // Destroy existing child
            GameObject childToDestroy = entryTextContainer.transform.GetChild(0).gameObject;
            Destroy(childToDestroy);
        }

        // Create new text
        TMP_Text newEntry = Instantiate(entry.GetEntryText(), entryTextContainer.transform);
        RectTransform rectTransform = newEntry.GetComponent<RectTransform>();

        // Make it fill the parent
        rectTransform.anchorMin = Vector2.zero;  // (0,0)
        rectTransform.anchorMax = Vector2.one;   // (1,1)
        rectTransform.offsetMin = Vector2.zero;   // Left, Bottom
        rectTransform.offsetMax = Vector2.zero;   // Right, Top
    }
}

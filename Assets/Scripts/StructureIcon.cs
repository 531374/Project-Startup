using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StructureIcon : MonoBehaviour
{
    [SerializeField] private Sprite icon; // Assign this in the Inspector
    public JournalEntry JournalEntry;

    private void Start ()
    {
        this.gameObject.tag = "Structure";
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
}

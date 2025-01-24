using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "JournalEntry", menuName = "Scriptables/Journal Entry", order =1)]
public class JournalEntry : ScriptableObject
{   
    public TMP_Text entryName;
    public TMP_Text entryText;
    public TMP_Text GetName ()
    {
        return entryName;
    }

    public TMP_Text GetEntryText ()
    {
        return entryText;
    }
}

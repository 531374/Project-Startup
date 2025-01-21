using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EntriesManager : MonoBehaviour
{
    private Transform entries;
    private float height;
    private float entryHeight;
    private List<GameObject> entryList= new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        entries = this.transform.GetChild (0);    
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < entries.childCount; i++)
        {
            GameObject entry = entries.GetChild (i).gameObject;

            if (entryList.Contains(entry)) continue;

            entryList.Add(entry);

            RectTransform rectTransform = entry.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // For UI elements with RectTransform
                entryHeight = rectTransform.rect.height;
                height += entryHeight;
            }
        }
   

        if (height != this.GetComponent<RectTransform> ().rect.height)
        {
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = height; 
            rectTransform.sizeDelta = sizeDelta;
        }
    }
}

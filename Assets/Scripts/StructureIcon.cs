using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureIcon : MonoBehaviour
{
    [SerializeField] private Sprite icon; // Assign this in the Inspector

    private void Start ()
    {
        this.gameObject.tag = "Structure";
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}

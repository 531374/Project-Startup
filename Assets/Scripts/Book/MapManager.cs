using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform terrainParent; // Parent object containing all terrains
    [SerializeField] private GameObject map;
    [SerializeField] private RectTransform mapRect;
    private Transform player;
    private Vector2 worldBoundsMin;
    private Vector2 worldBoundsMax;

    [Header("Settings")]
    [SerializeField] private float detectionRadius = 150f;

    [HideInInspector] public List<GameObject> visitedStructures = new List<GameObject> ();
    private List<GameObject> icons = new List<GameObject> ();

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Ship").transform;
        }

        Debug.Log (player);
        foreach (Transform child in map.transform)
        {
            icons.Add (child.gameObject);
        }
    }

    private void Update()
    {
        foreach (var structure in GameObject.FindGameObjectsWithTag("Structure"))
        {
            
            if (visitedStructures.Contains (structure)) continue;
            else if (Vector3.Distance(player.position, structure.transform.position) > detectionRadius)visitedStructures.Add (structure);
            
        }

        foreach (var structure in visitedStructures)
        {
            foreach (var icon in icons)
            {
                    if (icon.name == structure.GetComponent<Structure>().GetEntryName ().text)
                    {
                        icon.SetActive (true);
                    }
            }
        }
        
    }
}

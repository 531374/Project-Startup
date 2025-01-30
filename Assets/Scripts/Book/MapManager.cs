using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform terrainParent; // Parent object containing all terrains
    [SerializeField] private GameObject map;
    [SerializeField] private RectTransform mapRect;
    private Transform player;

    [Header("Settings")]
    [SerializeField] private float detectionRadius = 250f;

    [HideInInspector] public List<GameObject> visitedStructures = new List<GameObject> ();
    private List<GameObject> icons = new List<GameObject> ();

    private void Start()
    {
        foreach (Transform child in map.transform)
        {
            icons.Add (child.gameObject);
        }
    }

    private void Update()
    {
        
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                Debug.Log("Player found: " + player.name);
            }
        }

        if (player == null)
        {
            GameObject foundShip = GameObject.FindGameObjectWithTag("Ship");
            if (foundShip != null)
            {
                player = foundShip.transform;
                Debug.Log("Ship found: " + player.name);
            }
        }


        foreach (var structure in GameObject.FindGameObjectsWithTag("Structure"))
        {   
            if (visitedStructures.Contains (structure)) continue;
            else if (Vector3.Distance(player.position, structure.transform.position) > detectionRadius) 
            {
                visitedStructures.Add (structure);
            }
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

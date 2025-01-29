using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    private void Update()
    {
        foreach (var structure in GameObject.FindGameObjectsWithTag("Structure"))
        {
            if (visitedStructures.Contains (structure)) continue;
            else visitedStructures.Add (structure);
            
            if (Vector3.Distance(player.position, structure.transform.position) > detectionRadius) continue;

           
        }
    }
}

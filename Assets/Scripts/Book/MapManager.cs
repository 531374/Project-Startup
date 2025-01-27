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
        // Calculate the world bounds once at the start
        CalculateWorldBounds();
        player = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    private void Update()
    {
        foreach (var structure in GameObject.FindGameObjectsWithTag("Structure"))
        {
            if (visitedStructures.Contains (structure)) continue;
            else visitedStructures.Add (structure);
            
            if (Vector3.Distance(player.position, structure.transform.position) > detectionRadius) continue;

            Structure structureIcon = structure.GetComponent<Structure>();
            Sprite icon = structureIcon.GetIcon();

            // Create a new UI Image for the map icon
            GameObject iconObject = new GameObject("StructureIcon");
            RectTransform iconRect = iconObject.AddComponent<RectTransform>();
            UnityEngine.UI.Image iconImage = iconObject.AddComponent<UnityEngine.UI.Image>();
            iconImage.sprite = icon;

            // Set the icon as a child of the map
            iconObject.transform.SetParent(map.transform);

            // Translate structure's world position to map position
            Vector2 mapPosition = WorldToMapPosition(structure.transform.position);
            iconRect.anchoredPosition = mapPosition;

            // Optional: Set size or other UI properties
            iconRect.sizeDelta = new Vector2(20, 20); // Set icon size
            iconRect.localScale = Vector3.one; // Reset scale
        }
    }

    private Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        // Map the world position to the normalized range [0, 1] based on world bounds
        float normalizedX = (worldPosition.x - worldBoundsMin.x) / (worldBoundsMax.x - worldBoundsMin.x);
        float normalizedY = (worldPosition.z - worldBoundsMin.y) / (worldBoundsMax.y - worldBoundsMin.y); // Assuming Y in world space is Z in map

        // Scale the normalized position to map size
        float mapWidth = mapRect.sizeDelta.x;
        float mapHeight = mapRect.sizeDelta.y;

        float mapX = normalizedX * mapWidth;
        float mapY = normalizedY * mapHeight;

        return new Vector2(mapX, mapY);
    }

    private void CalculateWorldBounds()
    {
        // Initialize bounds to extreme values
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        // Loop through each terrain in the parent
        foreach (Transform child in terrainParent)
        {
            if (child.TryGetComponent<Terrain>(out Terrain terrain))
            {
                // Get the terrain's world bounds
                Vector3 terrainPosition = terrain.transform.position;
                Vector3 terrainSize = terrain.terrainData.size;

                Vector2 terrainMin = new Vector2(terrainPosition.x, terrainPosition.z);
                Vector2 terrainMax = new Vector2(terrainPosition.x + terrainSize.x, terrainPosition.z + terrainSize.z);

                // Update global bounds
                min = Vector2.Min(min, terrainMin);
                max = Vector2.Max(max, terrainMax);
            }
        }

        // Set world bounds
        worldBoundsMin = min;
        worldBoundsMax = max;
    }
}

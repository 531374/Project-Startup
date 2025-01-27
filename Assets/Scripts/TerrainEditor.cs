using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class TerrainEditor : MonoBehaviour
{
    Terrain terrain;
    TerrainData terrainData;

    private Transform player;

    float[,] baseHeights;

    Dictionary<Vector2Int, float> changedTerrainCoords;


    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        int resolution = terrainData.heightmapResolution;
        
        baseHeights = new float[resolution, resolution];
        baseHeights = terrainData.GetHeights(0, 0, resolution, resolution);


        changedTerrainCoords = new Dictionary<Vector2Int, float>();

        player = FindAnyObjectByType<ShipController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            terrainData.SetHeights(0, 0, baseHeights);
        }
    }

    private void FixedUpdate()
    {
        List<Vector2Int> removedItems = new List<Vector2Int>();

        foreach(KeyValuePair<Vector2Int, float> coord in changedTerrainCoords)
        {
            if (Time.time - coord.Value > 2.5f) 
            {
                float[,] oldHeight = new float[1, 1];
                oldHeight[0, 0] = baseHeights[coord.Key.y, coord.Key.x];
                terrainData.SetHeights(coord.Key.x, coord.Key.y, oldHeight);
                removedItems.Add(coord.Key);
            }
        }
        


        foreach (Vector2Int item in removedItems)
        {
            changedTerrainCoords.Remove(item);
        }
    }

    public void DeformTerrainAtPoint(Vector3 point)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrainData.size;
        
        float terrainX = (point.x - terrainPosition.x) / terrainData.size.x;
        float terrainZ = (point.z - terrainPosition.z) / terrainData.size.z;

        int testX = Mathf.RoundToInt(terrainX * (terrainData.heightmapResolution));
        int testZ = Mathf.RoundToInt(terrainZ * (terrainData.heightmapResolution));

        Vector2Int coord = new Vector2Int(testX, testZ);

        int resolution = terrainData.heightmapResolution;

        if (coord.x < 0 || coord.x >= resolution || coord.y < 0 || coord.y >= resolution)
        {
            return;
        }

        if (changedTerrainCoords.ContainsKey(coord)) return;

        changedTerrainCoords.Add(coord, Time.time);
        float[,] newHeight = new float[1, 1];
        newHeight[0,0] = terrainData.GetHeights(coord.x, coord.y, 1, 1)[0,0] - 0.0012f;
        terrainData.SetHeights(coord.x, coord.y, newHeight);
    }


    //DO NOT REMOVE OR DISABLE OR WHATEVER PLEASE
    private void OnDisable()
    {
        //ONCE AGAIN VERY IMPORTANT DO NOT REMOVE
        terrainData.SetHeights(0, 0, baseHeights);
    }
}
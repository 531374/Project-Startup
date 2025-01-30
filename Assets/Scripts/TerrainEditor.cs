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

        //After 2.5 seconds the terrain will be reset back to its original height
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
        int resolution = terrainData.heightmapResolution;
        
        //Get the coordinate of the relative to terrain (0.0 - 1.0)
        float terrainX = (point.x - terrainPosition.x) / terrainData.size.x;
        float terrainZ = (point.z - terrainPosition.z) / terrainData.size.z;

        //Convert it to terrain coords (0 - resolution)
        int testX = Mathf.RoundToInt(terrainX * resolution);
        int testZ = Mathf.RoundToInt(terrainZ * resolution);

        Vector2Int coord = new Vector2Int(testX, testZ);


        if (coord.x < 0 || coord.x >= resolution || coord.y < 0 || coord.y >= resolution)
        {
            return;
        }


        //Check if the coord is already changed
        if (changedTerrainCoords.ContainsKey(coord)) return;

        //Otherwise lower the terrain and keep track of it
        changedTerrainCoords.Add(coord, Time.time);
        float[,] newHeight = new float[1, 1];
        newHeight[0,0] = terrainData.GetHeights(coord.x, coord.y, 1, 1)[0,0] - 0.0020f;
        terrainData.SetHeights(coord.x, coord.y, newHeight);
    }


    //DO NOT REMOVE OR DISABLE OR WHATEVER PLEASE
    private void OnDisable()
    {
        //ONCE AGAIN VERY IMPORTANT DO NOT REMOVE
        terrainData.SetHeights(0, 0, baseHeights);
    }
}
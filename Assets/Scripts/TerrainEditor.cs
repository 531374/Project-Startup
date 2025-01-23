using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{

    [SerializeField] float test = 0.0001f;

    Terrain terrain;
    TerrainData terrainData;

    private Transform player;

    float[,] baseHeights;

    List<TerrainCoord> changedTerrainCoords;

    


    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        int resolution = terrainData.heightmapResolution;
        
        baseHeights = new float[resolution, resolution];
        baseHeights = terrainData.GetHeights(0, 0, resolution, resolution);

        changedTerrainCoords = new List<TerrainCoord>();

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
        List<TerrainCoord> removedItems = new List<TerrainCoord>();

        foreach(TerrainCoord coord in changedTerrainCoords)
        {
            if (Time.time - coord.Time > 2.0f) 
            {
                float[,] baseValue = new float[1, 1];
                baseValue[0, 0] = coord.OldHeight;
                terrainData.SetHeights(coord.Coord.x, coord.Coord.y, baseValue);
                removedItems.Add(coord);
            }
        }

        foreach (TerrainCoord item in removedItems)
        {
            changedTerrainCoords.Remove(item);
        }
    }

    public void DeformTerrainAtPoint(Vector3 point)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrainData.size;
        
        float terrainX = (point.x - terrainPosition.x) / terrainSize.x;
        float terrainZ = (point.z - terrainPosition.z) / terrainSize.z;

        int testX = Mathf.RoundToInt(terrainX * (terrainData.heightmapResolution - 1));
        int testZ = Mathf.RoundToInt(terrainZ * (terrainData.heightmapResolution - 1));

        TerrainCoord terrainCoord;
        terrainCoord.Time = Time.time;
        terrainCoord.Coord = new Vector2Int(testX, testZ);
        float[,] heights = terrainData.GetHeights(testX, testZ, 1, 1);
        terrainCoord.OldHeight = heights[0,0];

        if (terrainX < 0 || terrainX > 1 || terrainZ < 0 || terrainZ > 1)
        {
            Debug.Log("Invalid Position");
            return;
        }

        if (changedTerrainCoords.Contains(terrainCoord)) return;

        changedTerrainCoords.Add(terrainCoord);

        //float[,] heights = terrainData.GetHeights(testX, testZ, 1, 1);
        heights[0, 0] = terrainCoord.OldHeight * 0.9f;

        //float[,] newHeight = new float[1, 1];
        //newHeight[0, 0] = baseHeights[terrainCoord.x, terrainCoord.y] - test;

        terrainData.SetHeights(testX, testZ, heights);
    }


    //DO NOT REMOVE OR DISABLE OR WHATEVER PLEASE
    private void OnDisable()
    {

        //ONCE AGAIN VERY IMPORTANT DO NOT REMOVE
        terrainData.SetHeights(0, 0, baseHeights);
    }

    //float SampleHeightTexture(Vector2 uv)
    //{
    //    int x = Mathf.Clamp(Mathf.FloorToInt(uv.x * heightMap.width), 0, heightMap.width - 1);
    //    int y = Mathf.Clamp(Mathf.FloorToInt(uv.y * heightMap.height), 0, heightMap.height - 1);

    //    return heightMap.GetPixel(x, y).r;
    //}
}

struct TerrainCoord
{
    public Vector2Int Coord;
    public float Time;
    public float OldHeight;
}
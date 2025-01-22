using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{

    Terrain terrain;
    TerrainData terrainData;


    [SerializeField] Texture2D heightMap;

    [SerializeField] Transform test;

    [SerializeField] float heightMultiplier;

    float[,] baseHeights;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        int resolution = terrainData.heightmapResolution;
        
        baseHeights = new float[resolution, resolution];
        baseHeights = terrainData.GetHeights(0, 0, resolution, resolution);

    }





    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyHeightMap();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            terrainData.SetHeights(0, 0, baseHeights);
        }
    }

    void CopyMesh(int resolution)
    {
        MeshFilter meshFilter = transform.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
    }

    void ApplyHeightMap()
    {
        int resolution = terrainData.heightmapResolution;

        //float[,] heights = new float[resolution, resolution];

        for(int x = 0; x < resolution; x++)
        {
            for(int z = 0; z < resolution; z++)
            {
                float u = (float)x / resolution;
                float v = (float)z / resolution;

                //float height = SampleHeightTexture(u, v);
            }
        }
    }

    void DeformCloseTerrain(int radius)
    {

        Vector2Int closestPoint = ClosestTerrainPoint();
        List<Vector2> gridPoints = new List<Vector2>();

        for (int x = closestPoint.x - radius / 2; x <= closestPoint.x + radius / 2; x++)
        {
            for (int y = closestPoint.y - radius / 2; y <= closestPoint.y + radius / 2; y++)
            {
                if (x >= 0 && x < baseHeights.Length && y >= 0 && y < baseHeights.Length)
                {
                    
                }
            }
        }

       
    }

    private void OnDisable()
    {
        terrainData.SetHeights(0, 0, baseHeights);
    }

    Vector2Int ClosestTerrainPoint()
    {
        Vector3 playerPos = test.position;
        Vector2 relativePos = new Vector2(terrain.transform.position.x - playerPos.x, terrain.transform.position.z - playerPos.z);
        Vector2 terrainSize = terrainData.size;
        int heightMapSize = terrainData.heightmapResolution;

        int x = Mathf.RoundToInt((relativePos.x / terrainSize.x) * (heightMapSize - 1));
        int z = Mathf.RoundToInt((relativePos.y / terrainSize.y) * (heightMapSize - 1));

        return new Vector2Int(x, z);
    }

    float SampleHeightTexture(Vector2 uv)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt(uv.x * heightMap.width), 0, heightMap.width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(uv.y * heightMap.height), 0, heightMap.height - 1);

        return heightMap.GetPixel(x, y).r;
    }
}

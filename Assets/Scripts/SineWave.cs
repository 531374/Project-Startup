using UnityEngine;

public class SineWave : MonoBehaviour
{
    [Header("Wave Settings")]
    public float amplitude = 1f;       // Height of the wave
    public float wavelength = 2f;     // Length of the wave (distance between peaks)
    public float speed = 1f;          // Speed of wave movement

    [Header("Mesh Settings")]
    public bool applyToMesh = true;   // Should the wave deform a mesh
    public MeshFilter meshFilter;     // Reference to the mesh filter
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    void Start()
    {
        if (applyToMesh && meshFilter != null)
        {
            originalVertices = meshFilter.mesh.vertices;
            modifiedVertices = new Vector3[originalVertices.Length];
        }
    }

    void Update()
    {
        if (applyToMesh && meshFilter != null)
        {
            UpdateMeshWave();
        }
    }

    // Calculate the height of the wave at a given point
    public float GetWaveHeight(float x, float z)
    {
        float k = 2 * Mathf.PI / wavelength; // Wave number
        float omega = speed * k;            // Angular frequency
        return amplitude * Mathf.Sin(k * (x + z) - omega * Time.time);
    }

    // Update the mesh to follow the sine wave
    private void UpdateMeshWave()
    {
        Mesh mesh = meshFilter.mesh;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            vertex.y = GetWaveHeight(vertex.x + transform.position.x, vertex.z + transform.position.z);
            modifiedVertices[i] = vertex;
        }
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}

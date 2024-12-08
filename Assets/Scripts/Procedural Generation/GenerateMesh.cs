using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int numberOfObjectsToSpawn = 20;
    private List<Vector3> spawnPositions = new List<Vector3>();

    public int worldSizeX;
    public int worldSizeZ;

    public float noiseScale = .3f;
    public float heightScale = 2f;

    float minTerrainHeight;
    float maxTerrainHeight;

    Color[] colors;

    private Mesh mesh;

    private int[] triangles;
    private Vector3[] vertices;

    public Gradient gradient;

    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;

        MeshGenerator();
        UpdateMesh();

        SpawnObjects();
    }

    public void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            Vector3 spawnPosition = ObjectSpawnLocation();
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 ObjectSpawnLocation()
    {
        if (spawnPositions.Count == 0) return Vector3.zero;

        int randomIndex = Random.Range(0, spawnPositions.Count);
        Vector3 position = spawnPositions[randomIndex];
        position.y += 0.5f;
        spawnPositions.RemoveAt(randomIndex);
        return position;
    }

    void MeshGenerator()
    { 
        triangles = new int[worldSizeX * worldSizeZ * 6];
        vertices = new Vector3[(worldSizeX + 1) * (worldSizeZ + 1)];

        for (int i = 0, z = 0; z <= worldSizeZ; z++)
        {
            for (int x = 0; x <= worldSizeX; x++)
            {
                float y = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightScale;
                vertices[i] = new Vector3(x, y, z);

                if(y > maxTerrainHeight)
                    maxTerrainHeight = y;
                else if(y < minTerrainHeight)
                    minTerrainHeight = y;

                spawnPositions.Add(new Vector3(x, y, z));

                i++;
            }
        }

        int tris = 0;
        int verts = 0;

        for (int z = 0; z < worldSizeZ; z++)
        {
            for (int x = 0; x < worldSizeX; x++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + worldSizeZ + 1;
                triangles[tris + 2] = verts + 1;

                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + worldSizeZ + 1;
                triangles[tris + 5] = verts + worldSizeZ + 2;

                verts++;
                tris += 6;
            }
            verts++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= worldSizeZ; z++)
        {
            for (int x = 0; x <= worldSizeX; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
    
}
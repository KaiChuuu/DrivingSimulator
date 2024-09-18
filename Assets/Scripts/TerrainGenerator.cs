using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;
    private int xCenter;
    private int zCenter;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    public float noiseScale = 6f;
    public float noiseAmpZ = .01f;
    public float noiseAmpX = .01f;

    int density = 100;
    int minScale = 3;
    int maxScale = 6; //Exclusive, so its 5
    public GameObject treePrefab;
    public bool generateTrees = false;
    Vector3 originPos;
    int treeRadius = 5; //minimum radius between trees

    void Start()
    {
        originPos.x = transform.position.x;
        originPos.z = transform.parent.parent.transform.position.z;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        xCenter = xSize / 2;
        zCenter = zSize / 2;

        CreateShape();
        UpdateMesh();

        if (generateTrees)
        {
            GenerateTrees();
        }
    }

    void CreateShape()
    {
        CreateVertices();

        CreateTriangles();

        CreateGradient();
    }

    void CreateVertices()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = -zCenter; z <= zCenter; z++)
        {
            for (int x = -xCenter; x <= xCenter; x++)
            {
                float y = Mathf.PerlinNoise(x * noiseAmpX, z * noiseAmpZ) * noiseScale;

                float distanceX = Mathf.Abs(x + xCenter - xSize / 2f) / (xSize / 2f);     
  
                y *= Mathf.Lerp(1f, 0f, distanceX);
                
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight) maxTerrainHeight = y;
                if (y < minTerrainHeight) minTerrainHeight = y;

                i++;
            } 
        }
    }

    void CreateTriangles()
    {
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = -zCenter; z < zCenter; z++)
        {
            for (int x = -xCenter; x < xCenter; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void CreateGradient()
    {
        colors = new Color[vertices.Length];

        for (int i = 0, z = -zCenter; z <= zCenter; z++)
        {
            for(int x = -xCenter; x <= xCenter; x++)
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

    void GenerateTrees()
    {
        List<List<int>> positions = new List<List<int>>();

        for(int i=0; i < density; i++)
        {
            int sampleX = Random.Range(-xCenter, xCenter);
            int sampleZ = Random.Range(-zCenter, zCenter);

            List<int> coordinates = new List<int>();
            coordinates.Add(sampleX - treeRadius);
            coordinates.Add(sampleZ - treeRadius);
            coordinates.Add(sampleX + treeRadius);
            coordinates.Add(sampleZ + treeRadius);

            if (checkValidPosition(ref positions, ref coordinates))
            {
                positions.Add(coordinates);

                GameObject newTree = Instantiate(treePrefab, transform.parent);
                newTree.transform.position = new Vector3(sampleX + originPos.x, 0, sampleZ + originPos.z);
                newTree.transform.Rotate(Vector3.up, Random.Range(0, 360), Space.Self);
                int scale = Random.Range(minScale, maxScale);
                newTree.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    //Utilizing Leetcode Rectangle Overlap to check for valid positions
    bool checkValidPosition(ref List<List<int>> positions, ref List<int> current)
    {
        foreach(List<int> position in positions)
        {
            if ((position[2] > current[0] && position[3] > current[1]) &&
                (position[0] < current[2] && position[1] < current[3]))
            {
                return false;
            }
        }
        return true;
    }
}

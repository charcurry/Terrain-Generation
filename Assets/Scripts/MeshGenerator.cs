using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] verticies;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public Gradient gradient;

    private float minTerrainHeight;
    private float maxTerrainHeight;

    public float noiseMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
        ResetMinMax();
    }

    public void ResetMinMax()
    {
        minTerrainHeight = 0;
        maxTerrainHeight = 0;
    }

    private void CreateShape()
    {
        verticies = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((float)x * 0.3f, (float)z * 0.3f) * noiseMultiplier;
                verticies[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }
                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int tris = 0;
        int vert = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
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

        colors = new Color[verticies.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, verticies[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    // Update is called once per frame
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0;  i < verticies.Length; i++)
    //    {
    //        Gizmos.DrawSphere(verticies[i], 0.1f);
    //    }
    //}
}

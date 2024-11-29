using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public int Octaves = 6;
    public float Scale = 0.3f;

    public float offsetX = 0f;
    public float offsetY = 0f;

    public float Frequency_01 = 5f;
    public float FreqAmp_01 = 3f;

    public float Frequency_02 = 6f;
    public float FreqAmp_02 = 2.5f;

    public float Frequency_03 = 3f;
    public float FreqAmp_03 = 1.5f;

    public float Frequency_04 = 2.5f;
    public float FreqAmp_04 = 1f;

    public float Frequency_05 = 2f;
    public float FreqAmp_05 = .7f;

    public float Frequency_06 = 1f;
    public float FreqAmp_06 = .5f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

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
                float y = Calculate(x, z);
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

    float Calculate(float x, float z)
    {
        float[] octaveFrequencies = new float[] { Frequency_01, Frequency_02, Frequency_03, Frequency_04, Frequency_05, Frequency_06 };
        float[] octaveAmplitudes = new float[] { FreqAmp_01, FreqAmp_02, FreqAmp_03, FreqAmp_04, FreqAmp_05, FreqAmp_06 };
        float y = 0;

        for (int i = 0; i < Octaves; i++)
        {
            y += octaveAmplitudes[i] * Mathf.PerlinNoise(
                     octaveFrequencies[i] * x + offsetX * Scale,
                     octaveFrequencies[i] * z + offsetY * Scale);

        }

        return y;
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0;  i < verticies.Length; i++)
    //    {
    //        Gizmos.DrawSphere(verticies[i], 0.1f);
    //    }
    //}

    private void OnValidate()
    {
        if (Octaves < 1)
        {
            Octaves = 1;
        }
        if (Octaves > 6)
        {
            Octaves = 6;
        }
    }

}

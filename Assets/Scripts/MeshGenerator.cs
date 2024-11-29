using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    public bool drawGizmos = false;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize;
    public int zSize;

    public float offsetX;
    public float offsetY;

    public Gradient gradient;

    private float minTerrainHeight;
    private float maxTerrainHeight;

    [Header("Sliders")]
    [SerializeField]
    public Slider offsetXSlider;
    public Slider offsetYSlider;

    public Slider octavesSlider;
    public Slider scaleSlider;

    [Header("Octave Variables")]
    // Variables pertaining to octaves
    #region Octaves Variables
    public int octaves;
    public float scale;

    public float frequency1;
    public float amplitude1;

    public float frequency2;
    public float amplitude2;

    public float frequency3;
    public float amplitude3;

    public float frequency4;
    public float amplitude4;

    public float frequency5;
    public float amplitude5;

    public float frequency6;
    public float amplitude6;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Gets a random offset for the perlin noise algorythm to use
        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

        CreateMesh();
        UpdateMesh();
        GetSliders();
        SetSliders();
    }

    private void Update()
    {
        CreateMesh();
        UpdateMesh();
        ResetMinMax();
        UpdateSliders();
    }

    public void ResetMinMax()
    {
        minTerrainHeight = 0;
        maxTerrainHeight = 0;
    }

    // Creates the mesh using the other relevant methods
    private void CreateMesh()
    {
        CreateVerticies();

        CreateTriangles();

        GetColors();
    }

    // Creates the verticies given the x & zSize (the map size)
    void CreateVerticies()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = CalculateY(x, z);
                vertices[i] = new Vector3(x, y, z);

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
    }

    // Creates the triangles clockwise from each set of 3 verticies
    void CreateTriangles()
    {
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
    }
    
    // Gets the color the mesh is supposed to be at each elevation to look more like realistic terrain
    void GetColors()
    {
        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    // Updates mesh when changed
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    // Calculates the y value of the given vertex using its x, z coordinates
    float CalculateY(float x, float z)
    {
        float[] octaveFrequencies = new float[] { frequency1, frequency2, frequency3, frequency4, frequency5, frequency6 };
        float[] octaveAmplitudes = new float[] { amplitude1, amplitude2, amplitude3, amplitude4, amplitude5, amplitude6 };
        float y = 0;

        // for each octave perform the perlin noise algorythm on the pertaining x, y, frequency, amplitude, and scale value(s)
        for (int i = 0; i < octaves; i++)
        {
            y += octaveAmplitudes[i] * Mathf.PerlinNoise(
                     octaveFrequencies[i] * x + offsetX * scale,
                     octaveFrequencies[i] * z + offsetY * scale);

        }

        return y;
    }

    //Draws the verticies
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }

    void GetSliders()
    {
        offsetXSlider = GameObject.Find("OffsetXSlider").GetComponent<Slider>();
        offsetYSlider = GameObject.Find("OffsetYSlider").GetComponent<Slider>();

        octavesSlider = GameObject.Find("OctavesSlider").GetComponent<Slider>();
        scaleSlider = GameObject.Find("ScaleSlider").GetComponent<Slider>();
    }

    private void SetSliders()
    {
        offsetXSlider.value = offsetX;
        offsetYSlider.value = offsetY;

        octavesSlider.value = octaves;
        scaleSlider.value = scale;
    }

    private void UpdateSliders()
    {
        offsetX = offsetXSlider.value;
        offsetY = offsetYSlider.value;

        octaves = (int)octavesSlider.value;
        scale = scaleSlider.value;
    }

    // Makes sure values stay inside of a certain range to avoid errors
    private void OnValidate()
    {
        if (octaves < 1)
        {
            octaves = 1;
        }
        if (octaves > 6)
        {
            octaves = 6;
        }

        if (xSize < 1)
        {
            xSize = 1;
        }
        if (zSize < 1)
        {
            zSize = 1;
        }
    }

}

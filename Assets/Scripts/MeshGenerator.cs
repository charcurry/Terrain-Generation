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

    public Slider xSizeSlider;
    public Slider zSizeSlider;

    public Slider offsetXSlider;
    public Slider offsetYSlider;

    public Slider octavesSlider;
    public Slider scaleSlider;

    public Slider frequency1Slider;
    public Slider amplitude1Slider;
    public Slider frequency2Slider;
    public Slider amplitude2Slider;
    public Slider frequency3Slider;
    public Slider amplitude3Slider;
    public Slider frequency4Slider;
    public Slider amplitude4Slider;
    public Slider frequency5Slider;
    public Slider amplitude5Slider;
    public Slider frequency6Slider;
    public Slider amplitude6Slider;

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

    // Sets the sliders to the current values
    private void SetSliders()
    {
        xSizeSlider.value = xSize;
        zSizeSlider.value = zSize;

        offsetXSlider.value = offsetX;
        offsetYSlider.value = offsetY;

        octavesSlider.value = octaves;
        scaleSlider.value = scale;

        frequency1Slider.value = frequency1;
        amplitude1Slider.value = amplitude1;
        frequency2Slider.value = frequency2;
        amplitude2Slider.value = amplitude2;
        frequency3Slider.value = frequency3;
        amplitude3Slider.value = amplitude3;
        frequency4Slider.value = frequency4;
        amplitude4Slider.value = amplitude4;
        frequency5Slider.value = frequency5;
        amplitude5Slider.value = amplitude5;
        frequency6Slider.value = frequency6;
        amplitude6Slider.value = amplitude6;
    }

    // Updates the current values from the sliders
    private void UpdateSliders()
    {
        xSize = (int)xSizeSlider.value;
        zSize = (int)zSizeSlider.value;

        offsetX = offsetXSlider.value;
        offsetY = offsetYSlider.value;

        octaves = (int)octavesSlider.value;
        scale = scaleSlider.value;

        frequency1 = frequency1Slider.value;
        amplitude1 = amplitude1Slider.value;
        frequency2 = frequency2Slider.value;
        amplitude2 = amplitude2Slider.value;
        frequency3 = frequency3Slider.value;
        amplitude3 = amplitude3Slider.value;
        frequency4 = frequency4Slider.value;
        amplitude4 = amplitude4Slider.value;
        frequency5 = frequency5Slider.value;
        amplitude5 = amplitude5Slider.value;
        frequency6 = frequency6Slider.value;
        amplitude6 = amplitude6Slider.value;
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

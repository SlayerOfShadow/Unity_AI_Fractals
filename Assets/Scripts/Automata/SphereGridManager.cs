using UnityEngine;

public class SphereGridManager : MonoBehaviour
{
    public GameObject spherePrefab; // Sphere prefab
    public Automata cellularAutomaton; // Reference to your 2D cellular automaton script

    private GameObject[,,] sphereLayers; // Array to represent the sphere layers in 3D
    private int currentLayer = 0; // Current layer index

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        int width = cellularAutomaton.width;
        int height = cellularAutomaton.height;
        int layerCount = 41; // Number of layers including layer 0 to layer 40

        sphereLayers = new GameObject[layerCount, width, height]; // Adjusted for 3D

        // Initial Y position for the first layer
        float initialY = 0.0f;

        // Iterate through layers
        for (int layer = 0; layer < layerCount; layer++)
        {
            // Iterate through the grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Adjust the height based on the layer
                    Vector3 spherePosition = new Vector3(x, initialY + layer, y);

                    GameObject sphere = Instantiate(spherePrefab, spherePosition, Quaternion.identity);
                    sphereLayers[layer, x, y] = sphere;

                    // Make the sphere active or inactive based on the state of the cell
                    bool isAlive = cellularAutomaton.get_grid()[x, y] == 1;
                    sphere.SetActive(isAlive);
                }
            }
        }
    }

    void Update()
    {
        // Update the visual representation based on the state of the grid for all layers
        int width = cellularAutomaton.width;
        int height = cellularAutomaton.height;

        for (int layer = 0; layer < sphereLayers.GetLength(0); layer++) // Adjusted for 3D
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool isAlive = cellularAutomaton.get_grid()[x, y] == 1;
                    sphereLayers[layer, x, y].SetActive(isAlive);
                }
            }
        }

        // Increment the current layer and restart from layer 0 when reaching layer 40
        currentLayer = (currentLayer + 1) % sphereLayers.GetLength(0); // Adjusted for 3D
    }
}

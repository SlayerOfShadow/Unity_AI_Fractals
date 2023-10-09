using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public int width = 10; // Grid width
    public int height = 10; // Grid height
    public int depth = 10; // Grid depth

    public int densityDivider = 2;
    public int aliveNeighborsBirth = 2;
    public int aliveNeighborsDeathMin = 1;
    public int aliveNeighborsDeathMax = 3;




    public int[,,] grid; // 3D array to store cell states

    void InitializeGrid()
    {
        grid = new int[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Generate a random number (1 or 2)
                    int randomState = Random.Range(1, densityDivider);
                    if (randomState != 1) randomState = 2;

                    // Assign the random state to the cell
                    grid[x, y, z] = randomState;
                }
            }
        }
    }

    public void UpdateGrid()
    {
        // Create a copy of the current grid to calculate the new state
        int[,,] newGrid = new int[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Get the current cell's state
                    int currentState = grid[x, y, z];

                    // Count the number of alive neighbors
                    int aliveNeighbors = CountAliveNeighbors(x, y, z);

                    // Apply Conway's Game of Life rules
                    if (currentState == 1)
                    {
                        // Any live cell with fewer than 2 or more than 3 live neighbors dies
                        if (aliveNeighbors < aliveNeighborsDeathMin || aliveNeighbors > aliveNeighborsDeathMax)
                            newGrid[x, y, z] = 2; // Cell dies
                        else
                            newGrid[x, y, z] = 1; // Cell survives
                    }
                    else if (currentState == 2)
                    {
                        // Any dead cell with exactly 3 live neighbors becomes alive
                        if (aliveNeighbors == aliveNeighborsBirth)
                            newGrid[x, y, z] = 1; // Cell becomes alive
                        else
                            newGrid[x, y, z] = 2; // Cell remains dead
                    }
                }
            }
        }

        // Update the grid with the new state
        grid = newGrid;
    }

    int CountAliveNeighbors(int x, int y, int z)
    {
        int aliveCount = 0;

        // Define the neighbors' relative positions
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dz = { -1, -1, -1, -1, 1, 1, 1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            int nz = z + dz[i];

            // Check if the neighbor is within the grid boundaries
            if (nx >= 0 && nx < width && ny >= 0 && ny < height && nz >= 0 && nz < depth)
            {
                if (grid[nx, ny, nz] == 1)
                {
                    aliveCount++;
                }
            }
        }

        return aliveCount;
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrid();
    }
}

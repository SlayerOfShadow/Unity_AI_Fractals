using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // R�f�rence au prefab du cube


    private GameObject[,,] gridObjects; // Tableau pour stocker les objets de la grille
    private CellularAutomata cellularAutomaton; // Reference to your CellularAutomaton3D script

    void Start()
    {
        cellularAutomaton = GetComponent<CellularAutomata>();
        CreateGrid();
    }

    public float updateInterval = .0001f; // Mettre � jour toutes les 0.0001 secondes
    private float timeSinceLastUpdate = 0.0f;

    void Update()
    {
        // Accumuler le temps
        timeSinceLastUpdate += Time.deltaTime;

        // V�rifier s'il est temps de mettre � jour la grille
        if (timeSinceLastUpdate >= updateInterval)
        {
            // Mettre � jour la logique de la grille
            cellularAutomaton.UpdateGrid();

            // Mettre � jour les objets de la grille existants
            UpdateGridVisuals();

            // R�initialiser le compteur de temps
            timeSinceLastUpdate = 0.0f;
        }
    }

    void CreateGrid()
    {
        gridObjects = new GameObject[cellularAutomaton.width, cellularAutomaton.height, cellularAutomaton.depth];

        for (int x = 0; x < cellularAutomaton.width; x++)
        {
            for (int y = 0; y < cellularAutomaton.height; y++)
            {
                for (int z = 0; z < cellularAutomaton.depth; z++)
                {
                    int cellState = cellularAutomaton.grid[x, y, z];
                    Vector3 cellPosition = new Vector3(x, y, z);

                    if (cellState == 1)
                    {
                        GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                        gridObjects[x, y, z] = cell;
                    }
                }
            }
        }
    }

    void UpdateGridVisuals()
    {
        for (int x = 0; x < cellularAutomaton.width; x++)
        {
            for (int y = 0; y < cellularAutomaton.height; y++)
            {
                for (int z = 0; z < cellularAutomaton.depth; z++)
                {
                    int cellState = cellularAutomaton.grid[x, y, z];
                    GameObject cell = gridObjects[x, y, z];

                    if (cellState == 1)
                    {
                        // Activer le cube et ajuster sa position si n�cessaire
                        if (cell == null)
                        {
                            cell = Instantiate(cellPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
                            gridObjects[x, y, z] = cell;
                        }
                        // Vous pouvez �galement mettre � jour la couleur ici si n�cessaire
                    }
                    else
                    {
                        // D�sactiver le cube s'il est actuellement activ�
                        if (cell != null)
                        {
                            Destroy(cell);
                            gridObjects[x, y, z] = null;
                        }
                    }
                }
            }
        }
    }
}

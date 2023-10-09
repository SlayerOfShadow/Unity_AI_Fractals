using UnityEngine;

public class Automata : MonoBehaviour
{
    public int width = 10; // Largeur de la grille
    public int height = 10; // Hauteur de la grille
    public float cellSize = 1.0f; // Taille des cellules
    public GameObject cellPrefab; // Pr�fabriqu� de la cellule
    public float updateInterval = 1.0f; // Intervalle de mise � jour

    private int[,] grid; // Grille d'�tats des cellules (1 pour vivantes, 0 pour mortes)
    private GameObject[,] cellObjects; // Tableau d'objets pour repr�senter visuellement les cellules
    private float timeSinceLastUpdate = 0.0f;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new int[width, height];
        cellObjects = new GameObject[width, height];

        // Initialisation de la grille avec des cellules vivantes et mortes de mani�re al�atoire
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = Random.Range(0, 2);

                // Cr�ation d'une cellule visuelle
                Vector3 cellPosition = new Vector3(x * cellSize, 0.0f, y * cellSize);
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cellObjects[x, y] = cell;
                cell.SetActive(grid[x, y] == 1); // Active ou d�sactive la cellule en fonction de son �tat
            }
        }
    }

    public int[,] get_grid()
    {
        return grid;
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        // V�rification s'il est temps de mettre � jour la grille
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateGrid();
            timeSinceLastUpdate = 0.0f;
        }
    }

    void UpdateGrid()
    {
        int[,] newGrid = new int[width, height];

        // Appliquer les r�gles du jeu de la vie de Conway
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbors = CountAliveNeighbors(x, y);

                if (grid[x, y] == 1)
                {
                    if (neighbors < 2 || neighbors > 3)
                    {
                        newGrid[x, y] = 0; // La cellule meurt
                    }
                    else
                    {
                        newGrid[x, y] = 1; // La cellule survit
                    }
                }
                else
                {
                    if (neighbors == 3)
                    {
                        newGrid[x, y] = 1; // Une cellule morte rena�t
                    }
                }

                // Mettre � jour la repr�sentation visuelle en fonction de l'�tat de la cellule
                cellObjects[x, y].SetActive(newGrid[x, y] == 1);
            }
        }

        // Mettre � jour la grille avec les nouvelles valeurs
        grid = newGrid;
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Ignorer la cellule elle-m�me

                int neighborX = x + i;
                int neighborY = y + j;

                // V�rifier si le voisin est � l'int�rieur de la grille et s'il est vivant
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    count += grid[neighborX, neighborY];
                }
            }
        }

        return count;
    }
}

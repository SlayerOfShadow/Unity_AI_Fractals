using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float size = 1;
    [SerializeField] int iterations = 5;
    [SerializeField] int columns = 20;
    [SerializeField] int rows = 20;
    [SerializeField] float speed = 5;
    GameObject[,] cubes;
    int[,] cells;
    int[,] cells2;
    float lastFrameTime = 0;
    int count = 0;

    void Start()
    {
        lastFrameTime = Time.time;

        CreateGrid();
        ResetSketch();
    }

    void CreateGrid()
    {
        cubes = new GameObject[columns, rows];
        cells = new int[columns, rows];
        cells2 = new int[columns, rows];
    }

    void ResetSketch()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (i == 0 || j == 0 || i == columns - 1 || j == rows - 1)
                {
                    cells[i, j] = 0;
                }
                else
                {
                    cells[i, j] = Random.Range(0, 2);
                }
                cells2[i, j] = 0;
            }
        }
    }

    void Evolution()
    {
        for (int i = 1; i < columns - 1; i++)
        {
            for (int j = 1; j < rows - 1; j++)
            {
                Neighbors(i, j);
            }
        }
    }

    void Neighbors(int x, int y)
    {
        int count = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                count += cells[i, j];
            }
        }

        count -= cells[x, y];

        if (cells[x, y] == 1)
        {
            if (count < 2 || count > 3)
            {
                cells2[x, y] = 0;
            }
            else
            {
                cells2[x, y] = 1;
            }
        }
        else
        {
            if (count == 3)
            {
                cells2[x, y] = 1;
            }
            else
            {
                cells2[x, y] = 0;
            }
        }
    }

    void Update()
    {
        if (count < iterations &&Time.time - lastFrameTime > 1f / speed)
        {
            lastFrameTime = Time.time;

            Evolution();

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (cells2[i, j] == 1)
                    {
                        GameObject obj = Instantiate(prefab, new Vector3(i * size, count * size, j * size), Quaternion.identity, gameObject.transform);
                        obj.transform.localScale = new Vector3(size, size, size);
                        cubes[i, j] = obj;
                    }
                }
            }
            
            int[,] temp = cells;
            cells = cells2;
            cells2 = temp;
            count++;
        }
    }
}

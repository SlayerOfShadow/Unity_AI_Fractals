using UnityEngine;
using System.Collections.Generic;

public class GameOfLife : MonoBehaviour
{
    [SerializeField]
    TrueBridge trueBridge;
    Vector3 direction;
    public bool canBuild = true;
    [SerializeField] GameObject prefab;
    [SerializeField] float size = 1;
    public int maxIterations = 5;
    [SerializeField] int columns = 20;
    [SerializeField] int rows = 20;
    GameObject[,] cubes;
    int[,] cells;
    int[,] cells2;
    float lastFrameTime = 0;
    public int count = 0;
    Vector3 startPosition;
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    MeshFilter meshFilter;
    Vector3 objStartPosition;
    Quaternion startRotation;

    void Start()
    {
        direction = trueBridge.direction;

        meshFilter = GetComponent<MeshFilter>();
        objStartPosition = transform.position;
        startRotation = transform.rotation;

        lastFrameTime = Time.time;
        startPosition = transform.position;
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

    public void Build()
    {
        if (count < maxIterations)
        {
            Vector3 v = new Vector3();

            Evolution();

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (cells2[i, j] == 1)
                    {
                        if (direction.x != 0) v = new Vector3(direction.x * -count * size + startPosition.x, j * size + startPosition.y, i * size + startPosition.z);
                        else if (direction.y != 0) v = new Vector3(j * size + startPosition.x, direction.y * -count * size + startPosition.y, i * size + startPosition.z);
                        else if (direction.z != 0) v = new Vector3(j * size + startPosition.x, i * size + startPosition.y, direction.z * -count * size + startPosition.z);
                        GameObject obj = Instantiate(prefab, v, Quaternion.identity, gameObject.transform);
                        obj.transform.localScale = new Vector3(size, size, size);
                        cubes[i, j] = obj;
                        meshFilters.Add(obj.transform.GetComponent<MeshFilter>());
                    }
                }
            }

            int[,] temp = cells;
            cells = cells2;
            cells2 = temp;
            count++;
        }
        else
        {
            //if (canBuild) CombineMesh();
            canBuild = false;
        }
    }

    void CombineMesh()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].mesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        Mesh temp = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        temp.CombineMeshes(combine);
        meshFilter.mesh = temp;

        transform.position = startPosition;
        transform.rotation = startRotation;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}

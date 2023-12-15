using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SpatialGrid : MonoBehaviour
{
    private Dictionary<Vector2Int, List<Character>> grid;

    public SpatialGrid(float cellSize)
    {
        grid = new Dictionary<Vector2Int, List<Character>>();
        CellSize = cellSize;
    }

    public float CellSize { get; private set; }

    public void AddObject(Character obj)
    {
        Vector2Int cell = GetCell(obj.transform.position);
        if (!grid.TryGetValue(cell, out List<Character> objectsInCell))
        {
            objectsInCell = new List<Character>();
            grid[cell] = objectsInCell;
        }

        objectsInCell.Add(obj);
    }

    public List<Character> GetObjectsInCell(Vector2Int cell)
    {
        if (grid.TryGetValue(cell, out List<Character> objectsInCell))
        {
            return objectsInCell;
        }

        return null;
    }

    public Vector2Int GetCell(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / CellSize);
        int z = Mathf.FloorToInt(position.z / CellSize);
        return new Vector2Int(x, z);
    }
}

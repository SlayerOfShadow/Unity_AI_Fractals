using UnityEngine;
using System.Collections.Generic;

public class MengerSponge : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float size = 10.0f;
    [SerializeField] int iterations = 2;
    [SerializeField] float offset = 1;
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    MeshFilter meshFilter;
    Vector3 startPosition;
    Quaternion startRotation;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        prefab.transform.localScale = new Vector3(size, size, size);
        GenerateMengerSponge(transform.position, size, iterations);
        CombineMesh();
    }

    void GenerateMengerSponge(Vector3 position, float size, int iterations)
    {
        if (iterations == 0)
        {
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, gameObject.transform);
            obj.transform.localScale = new Vector3(size, size, size);
            meshFilters.Add(obj.transform.GetComponent<MeshFilter>());
            Destroy(obj);
            return;
        }

        float subSize = size / 3.0f;
        float[] offsets = { -1, 0, 1 };

        foreach (float xOffset in offsets)
        {
            foreach (float yOffset in offsets)
            {
                foreach (float zOffset in offsets)
                {
                    if (Mathf.Abs(xOffset) + Mathf.Abs(yOffset) + Mathf.Abs(zOffset) != 0
                        && Mathf.Abs(xOffset) + Mathf.Abs(yOffset) + Mathf.Abs(zOffset) != 1)
                    {
                        Vector3 newPosition = position + new Vector3(xOffset * subSize, yOffset * subSize, zOffset * subSize) * offset;
                        GenerateMengerSponge(newPosition, subSize, iterations - 1);
                    }
                }
            }
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
    }
}

using UnityEngine;
using System.Collections.Generic;

public class SierpinskiPyramid : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float size = 10.0f;
    [SerializeField] int iterations = 2;
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    MeshFilter meshFilter;
    Vector3 startPosition;
    Quaternion startRotation;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        GenerateSierpinskiPyramid(transform.position, size, iterations);
        CombineMesh();
    }

    void GenerateSierpinskiPyramid(Vector3 position, float size, int iterations)
    {
        if (iterations == 0)
        {
            // Create and position the pyramid at the specified location
            GameObject pyramid = Instantiate(prefab, position, Quaternion.identity, gameObject.transform);
            pyramid.transform.localScale = new Vector3(size, size, size);
            meshFilters.Add(pyramid.transform.GetComponent<MeshFilter>());
            Destroy(pyramid);
            return;
        }
        else
        {
            float r = size / 4.0f;
            float h = size / 2.0f;
            Vector3[] childOffsets = {
                new Vector3(0, h / 2, 0),
                new Vector3(r, -h / 2, r),
                new Vector3(r, -h / 2, -r),
                new Vector3(-r, -h / 2, r),
                new Vector3(-r, -h / 2, -r)
            };

            foreach (Vector3 offset in childOffsets)
            {
                Vector3 newPosition = position + offset;
                float newSize = size / 2.0f;
                GenerateSierpinskiPyramid(newPosition, newSize, iterations - 1);
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
        Mesh temp = new Mesh();
        temp.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        temp.CombineMeshes(combine);
        meshFilter.mesh = temp;

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}

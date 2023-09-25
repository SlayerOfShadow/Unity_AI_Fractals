using UnityEngine;
using System.Collections.Generic;

public class FractalGenerator : MonoBehaviour
{
    public GameObject prefab;
    public int iterations = 3;
    public float scaleDownFactor = 0.5f;
    private HashSet<Vector3> instantiatedPositions = new HashSet<Vector3>();
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    MeshFilter meshFilter;
    Vector3 startPosition;
    Quaternion startRotation;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        meshFilters.Add(meshFilter);
        GenerateFractal(transform, iterations);
        CombineMesh();
    }

    void GenerateFractal(Transform parent, int iteration)
    {
        if (iteration == 0)
            return;

        MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 worldVertex = parent.TransformPoint(vertex);

                // Check if this position has already been instantiated
                if (!instantiatedPositions.Contains(worldVertex))
                {
                    GameObject childPrefab = Instantiate(prefab, worldVertex, Quaternion.identity, parent);
                    childPrefab.transform.localScale *= scaleDownFactor;
                    instantiatedPositions.Add(worldVertex);
                    meshFilters.Add(childPrefab.transform.GetComponent<MeshFilter>());
                    Destroy(childPrefab);
                    GenerateFractal(childPrefab.transform, iteration - 1);
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
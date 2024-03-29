using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigLSystemTree : MonoBehaviour
{
    public int iterations = 1;
    [SerializeField] float scale = 0.5f;
    [SerializeField] Vector2 randomOffset = new Vector2(2, 8);
    [SerializeField] Vector2 randomAngle = new Vector2(90, 90);
    [SerializeField] GameObject prefab; // Reference to a branch prefab.
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    MeshFilter meshFilter;
    Vector3 startPosition;
    Quaternion startRotation;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        GenerateTree(transform.position, Vector3.up, iterations, prefab.transform.localScale.y);
        CombineMesh();
    }

    void GenerateTree(Vector3 position, Vector3 direction, int iterations, float length)
    {
        if (iterations == 0)
            return;

        // Create a new branch at the current position and rotation.
        GameObject branch = Instantiate(prefab, position, Quaternion.identity, gameObject.transform);
        branch.transform.up = direction;
        branch.transform.localScale *= length;
        for (int i = 0; i < branch.transform.childCount; i++)
        {
            // Get the child at index 'i'
            Transform child = branch.transform.GetChild(i);

            // Check if the child has a MeshFilter component
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();

            // If a MeshFilter component is found, add it to the list
            if (meshFilter != null)
            {
                meshFilters.Add(meshFilter);
            }
        }
        meshFilters.Add(branch.transform.GetChild(0).GetComponent<MeshFilter>());
        Destroy(branch);

        for (int i = 0; i < 4; i++)
        {
            float angle = Random.Range(randomAngle.x, randomAngle.y);
            angle = i % 2 == 1 ? -angle : angle;
            Vector3 axis = i > 1 ? Vector3.forward : Vector3.right;
            Vector3 newDirection = Quaternion.AngleAxis(angle, axis) * direction;
            GenerateTree(position + direction * length * Random.Range(randomOffset.x, randomOffset.y), newDirection, iterations - 1, length * scale);
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

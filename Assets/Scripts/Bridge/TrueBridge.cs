using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueBridge : MonoBehaviour
{
    public GameObject cubePrefab;
    public int iterations = 0;

    public void Build()
    {
        Instantiate(cubePrefab, new Vector3(transform.position.x - (0.5f * iterations), transform.position.y, transform.position.z), Quaternion.identity, transform);
        iterations += 1;
    }
}

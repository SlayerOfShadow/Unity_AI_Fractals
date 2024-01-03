using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueBridge : MonoBehaviour
{
    public GameObject cubePrefab;
    private int iterations = 0;

    public void Build()
    {
        for (int i = 0; i < 4; i++)
        {
            Instantiate(cubePrefab, new Vector3(transform.position.x - (0.5f * iterations), transform.position.y, transform.position.z + 0.5f * i), Quaternion.identity, transform);
        }
        iterations += 1;
    }
}

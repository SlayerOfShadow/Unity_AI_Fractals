using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueBridge : MonoBehaviour
{
    public Vector3 direction;
    public GameObject cubePrefab;
    public int iterations = 0;

    public void Build()
    {
        if (direction.x != 0) cubePrefab.transform.localScale = new Vector3(0.5f, 0.5f, 2f);
        else if (direction.z != 0) cubePrefab.transform.localScale = new Vector3(2f, 0.5f, 0.5f);
        Instantiate(cubePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z) - (0.5f * iterations) * direction, Quaternion.identity, transform);
        iterations += 1;
    }
}

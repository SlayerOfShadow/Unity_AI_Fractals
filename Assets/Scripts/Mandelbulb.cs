using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mandelbulb : MonoBehaviour
{
    public Material material; // Drag and drop the material using your shader here in the Unity Inspector.
    private float powerValue = 10.0f; // Initial POWER value.

    // Update is called once per frame
    void Update()
    {
        // Calculate the new POWER value based on your animation logic.
        powerValue = Mathf.PingPong(Time.time, 10); // Example: Animating between 1 and 10.

        // Update the shader property with the new POWER value.
        material.SetFloat("_Power", powerValue);
    }
}

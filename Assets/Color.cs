using UnityEngine;

public class TrailColorToLight : MonoBehaviour
{
    public Light linkedLight; // Reference to the light component linked to your trail.

    private TrailRenderer trailRenderer;

    private void Start()
    {
        // Ensure that we have a valid reference to the TrailRenderer component.
        trailRenderer = GetComponent<TrailRenderer>();

        // Check if we have a valid reference to the light component.
        if (linkedLight == null)
        {
            Debug.LogWarning("Light component is not assigned. Please assign it in the Inspector.");
        }
    }

    private void Update()
    {
        // Check if we have a valid reference to the TrailRenderer and the Light.
        if (trailRenderer != null && linkedLight != null)
        {
            // Get the color from the TrailRenderer's material.
            Color trailColor = trailRenderer.material.GetColor("_ColorStart"); // "_Color" is the default property name for the color in Unity's standard shader.

            // Set the same color to the linked light.
            linkedLight.color = trailColor;
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class ModifyMultiplePostProcesses : MonoBehaviour
{
    public List<PostProcessVolume> postProcessVolumes = new List<PostProcessVolume>();
    public List<float> animationDurations = new List<float>();
    public List<float> maxWeights = new List<float>();
    public List<float> minWeights = new List<float>();

    private List<float> initialWeights;

    private void Start()
    {
        // Find and store references to all Post Process Volumes in the prefab
        PostProcessVolume[] volumes = GetComponentsInChildren<PostProcessVolume>();
        foreach (var volume in volumes)
        {
            postProcessVolumes.Add(volume);
        }

        // Initialize lists for animation parameters
        initialWeights = new List<float>() { 0.1f, 0.1f, 0.1f };
        animationDurations = new List<float>() { 10, 35, 21 };
        maxWeights = new List<float>() {  1, 0.94f, 1 };
        minWeights = new List<float>() { 0f, 0.1f, 0f};

        for (int i = 0; i < postProcessVolumes.Count; i++)
        {
            // Store the initial weights
            initialWeights.Add(postProcessVolumes[i].weight);

            // Initialize animation parameters for each volume (customize as needed)
            animationDurations.Add(animationDurations[i]);
            maxWeights.Add(maxWeights[i]);
            minWeights.Add(minWeights[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < postProcessVolumes.Count; i++)
        {
            float t = Mathf.Sin(2 * Mathf.PI * Time.time / animationDurations[i]);
            float newWeight = Mathf.Lerp(minWeights[i], maxWeights[i], (t + 1) / 2);
            postProcessVolumes[i].weight = newWeight;
        }
    }

    // Reset the weights to their initial values
    public void ResetWeights()
    {
        for (int i = 0; i < postProcessVolumes.Count; i++)
        {
            postProcessVolumes[i].weight = initialWeights[i];
        }
    }
}

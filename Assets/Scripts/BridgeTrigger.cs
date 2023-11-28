using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    [SerializeField] GameOfLife bridge;
    [SerializeField] string creatureTag;
    [SerializeField] int buildMultiplier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == creatureTag)
        {
            for (int i = 0; i < buildMultiplier; i++)
            {
                bridge.Build();
                print("build");
            }
        }
    }
}

using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    [SerializeField] GameOfLife bridge;
    [SerializeField] string creatureTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == creatureTag)
        {
            bridge.Build();
            print("build");
        }
    }
}

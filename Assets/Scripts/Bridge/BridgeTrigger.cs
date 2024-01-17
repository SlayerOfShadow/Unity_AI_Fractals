using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    [SerializeField] GameOfLife bridge;
    [SerializeField] TrueBridge trueBridge;
    [SerializeField] string creatureTag;
    public int buildMultiplier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == creatureTag)
        {
            if (other.GetComponent<MlAgent>().ressource == true)
            {
                int strenght = 2 * (other.GetComponent<MlAgent>().strenght) + 200;
                for (int i = 0; i < strenght; i++)
                {
                    bridge.Build();
                    if (bridge.count < bridge.maxIterations) trueBridge.Build();
                }
            }
        }
    }
}

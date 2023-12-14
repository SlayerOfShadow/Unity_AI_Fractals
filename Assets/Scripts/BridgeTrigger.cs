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
                for (int i = 0; i < buildMultiplier; i++)
                {
                    bridge.Build();
                    trueBridge.Build();
                }
            }
        }
    }
}

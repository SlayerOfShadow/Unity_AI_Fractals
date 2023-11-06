using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(destination);
        }
    }

}

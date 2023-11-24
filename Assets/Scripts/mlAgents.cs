using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class MlAgent : Agent
{
    private bool ressource = false;
    private Vector3 initialAgentPosition;

    public GeneticAlgorithm trees;
    public Terrain terrain;
    public GameObject bridge;


    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent to its initial position.
        float randomXOffset = Random.Range(-20f, 20f);
        float randomZOffset = Random.Range(-20f, 20f);

        // Set the new position with the random offsets
        Vector3 newPosition = initialAgentPosition + new Vector3(randomXOffset, 0f, randomZOffset);
        transform.position = newPosition;
        float randomYRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);


        ressource = false;

    }



    private int[] GetClosestTreeIndices(Vector3 agentPosition, Vector3[] treePositions, int numClosestTrees)
    {
        // Create a list to store distances and indices
        List<(float distance, int index)> distancesAndIndices = new List<(float, int)>();

        // Calculate distances and store them along with the tree index
        for (int i = 0; i < treePositions.Length; i++)
        {
            float distance = Vector3.Distance(agentPosition, treePositions[i]);
            distancesAndIndices.Add((distance, i));
        }

        // Sort the list by distance in ascending order
        distancesAndIndices.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Get the indices of the N closest trees
        int[] closestTreeIndices = distancesAndIndices.Take(numClosestTrees).Select(pair => pair.index).ToArray();

        return closestTreeIndices;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation.eulerAngles.y / 360f);

        int resolution = 10; 
        TerrainData terrainData = terrain.terrainData;

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float normalizedX = i / (float)resolution;
                float normalizedZ = j / (float)resolution;

             Vector3 worldPosition = new Vector3(
                normalizedX * terrainData.size.x,
                terrainData.GetInterpolatedHeight(normalizedX, normalizedZ),
                normalizedZ * terrainData.size.z
                );

                sensor.AddObservation(worldPosition);
            }
        }



        sensor.AddObservation(ressource ? 1f : 0f);

        if (ressource) { 
            sensor.AddObservation(bridge.transform.position);

        }
        else { 
            Vector3[] treePositions = trees.treeObjects.Select(tree => tree.transform.position).ToArray();
            int[] closestTreeIndices = GetClosestTreeIndices(transform.position, treePositions, 4);


            for (int i = 0; i < 4; i++)
            {
                // Make sure to handle cases where there are fewer than four trees
                if (i < closestTreeIndices.Length)
                {
                    sensor.AddObservation(treePositions[closestTreeIndices[i]]);
                }
                else
                {
                    // If there are fewer than four trees, add a placeholder observation (you can customize this based on your needs)
                    sensor.AddObservation(Vector3.zero);
                }
            }
        }


    }




    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotateY = actions.ContinuousActions[1];
      

        float moveSpeed = 10f;
        float rotationSpeed = 100f;

        // Move forward/backward
        Vector3 moveDirection = new Vector3(0, 0, move);
        transform.position += transform.TransformDirection(moveDirection) * Time.deltaTime * moveSpeed;

        // Rotate around the Y-axis
        transform.Rotate(Vector3.up, rotateY * Time.deltaTime * rotationSpeed);
    }
    public override void Heuristic(in ActionBuffers ActionsOut)
    {
        ActionSegment<float> continuousActions = ActionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }
    private void OnTriggerEnter(Collider other)
    {

        if (ressource)
        {
            if (other.CompareTag("bridge"))
            {
                SetReward(4f);
                Debug.Log("bridge");
                ressource = false;


            }
        }
        else
        {
            if (other.CompareTag("tree"))
            {
                SetReward(2f);
                Debug.Log("tree");
                ressource = true;

            }
        }
        
        if (other.CompareTag("water") )
        {
            SetReward(-2f);
            Debug.Log("water");

            EndEpisode();

        }
    }
        private void Awake()
    {
        // Store the initial positions of the agent and targets.
        initialAgentPosition = transform.position;

    }
}
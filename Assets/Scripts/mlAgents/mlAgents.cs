using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;


public class MlAgent : Agent
{

    private Character character;
    private Character.CapacitiesStatistics capacitiesStatistics;
    private bool previousCanMakeABaby = false;




    public int speed = 0;
    public int strenght = 0;
    public int vision = 0;


    public GameOfLife Gol1;
    public GameOfLife Gol2;
    public GameOfLife Gol3;
    public GameOfLife Gol4;

    public GeneticAlgorithm trees1;
    public GeneticAlgorithm trees2;
    public GeneticAlgorithm trees3;
    public GeneticAlgorithm trees4;

    public GameObject bridge1;
    public GameObject bridge2;
    public GameObject bridge3;
    public GameObject bridge4;


    public bool ressource = false;
    private Vector3 initialAgentPosition;
    private bool mort = false;
    private Rigidbody rb;



    void Start()
    {
        character = GetComponent<Character>();
        // Now you can use capacities
        capacitiesStatistics = character.GetIndividual().GetStatistics();

        speed = capacitiesStatistics.speed;
        vision = capacitiesStatistics.vision;
        strenght = capacitiesStatistics.strength;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.rotation = Quaternion.Euler(0f,0f, 0f);
    }

    public override void OnEpisodeBegin()
    {
                        // Reset the agent to its initial position.
                        float randomXOffset = Random.Range(-20f, 20f);
                        float randomZOffset = Random.Range(-20f, 20f);

                        // Set the new position with the random offsets
                        Vector3 newPosition = initialAgentPosition + new Vector3(randomXOffset, 0f, randomZOffset);
                        if(mort)
                        transform.position = newPosition;



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
        if (Gol1 != null && !Gol1.canBuild)
        {
            Debug.Log("drip");
        }
        if (bridge1 != null && trees1!=null) { 
          

      
            if (ressource)
            {
                sensor.AddObservation(bridge1.transform.position);
                Debug.Log(bridge1.transform.position);


            }
            else
            {
                Vector3[] treePositions = trees1.treeObjects.Select(tree => tree.transform.position).ToArray();
                int[] closestTreeIndices = GetClosestTreeIndices(transform.position, treePositions, 4);


                for (int i = 0; i < 7 - vision; i++)
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

        if (character != null)
        {
            bool currentCanMakeABaby = character.CanMakeABaby();

            if (currentCanMakeABaby != previousCanMakeABaby && currentCanMakeABaby== false)
            {
                // CanMakeABaby state changed from true to false
                SetReward(1f); // Add a negative reward for the change
                Debug.Log("sex");

            }
            sensor.AddObservation(character.CanMakeABaby());
            previousCanMakeABaby = currentCanMakeABaby;

        }
        sensor.AddObservation(speed);
        sensor.AddObservation(vision);
        sensor.AddObservation(strenght);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation.eulerAngles.y / 360f);
        sensor.AddObservation(ressource ? 1f : 0f);

        


    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotateY = actions.ContinuousActions[1];

        float moveSpeed = 6f* (2f+(float)speed);
        float rotationSpeed = 100f;

        // Interpolate movement for smoother transitions
        float interpolationFactor = 10f; // You can adjust this value based on your preference
        Vector3 moveDirection = new Vector3(0, 0, move);
        Vector3 newPosition = transform.position + transform.TransformDirection(moveDirection) * Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * interpolationFactor);

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
                if (ressource == false)
                {
                    LSystemTree lSystemTree = other.GetComponent<LSystemTree>();
                    if (lSystemTree.currentHealthPoint == 1 && trees1.treeObjects != null) trees1.treeObjects.Remove(other.gameObject);
                    if (lSystemTree!=null) lSystemTree.LooseHealthPoint();
                }
                ressource = true;
                print("loose life");
            }
        }
        
        if (other.CompareTag("water") )
        {
            SetReward(-2f);
            Debug.Log("water");
            mort = true;
            EndEpisode();

        }
    }
        private void Awake()
    {
        // Store the initial positions of the agent and targets.
        initialAgentPosition = transform.position;

    }
}
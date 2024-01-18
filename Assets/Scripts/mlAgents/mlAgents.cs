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

    public GameObject water;

    public GameObject[] spawnArray = new GameObject[4];
    public GameObject[] endOfBridgeArray = new GameObject[4];
    public GameObject[] bridgeTriggerArray = new GameObject[4];
    public GameOfLife[] gameOfLifeArray = new GameOfLife[4];
    public GeneticAlgorithm[] GeneticAlgorithmArray = new GeneticAlgorithm[4];

    public int ile= 0 ;
    public int etat = 0;

    public bool ressource = false;
    private bool mort = false;
    private Rigidbody rb;
    private Vector3 prevPos;



    void Start()
    {
        character = GetComponent<Character>();
        // Now you can use capacities
        capacitiesStatistics = character.GetIndividual().GetStatistics();


        //attribiut les stat au personnage
        speed = capacitiesStatistics.speed;
        vision = capacitiesStatistics.vision;
        strenght = capacitiesStatistics.strength;


        // fix ses rotations 
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        

      

    


        
       


    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent to its initial position.
        float randomXOffset = Random.Range(-3f, 3f);
        float randomZOffset = Random.Range(-3f, 3f);

        // Set the new position with the random offsets
        Vector3 newPosition = spawnArray[ile].transform.position + new Vector3(randomXOffset, 0f, randomZOffset);
        if (mort) { 
            transform.position = newPosition;
            mort = false;
        }

        ressource = false;
    }



    public override void CollectObservations(VectorSensor sensor)
    {


        sensor.AddObservation(etat);
        sensor.AddObservation(ile);
        sensor.AddObservation(water.transform.position.y);


        sensor.AddObservation(speed);
        sensor.AddObservation(vision);
        sensor.AddObservation(strenght);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation.eulerAngles.y / 360f);
        sensor.AddObservation(ressource ? 1f : 0f);



        if (character != null)
        {
            bool currentCanMakeABaby = character.CanMakeABaby();

            if (currentCanMakeABaby != previousCanMakeABaby && currentCanMakeABaby == false)
            {
                // CanMakeABaby state changed from true to false
                SetReward(1f); // Add a negative reward for the change
                Debug.Log("sex");

            }
            sensor.AddObservation(character.CanMakeABaby());
            previousCanMakeABaby = currentCanMakeABaby;

        }


        if (gameOfLifeArray[ile] != null && !gameOfLifeArray[ile].canBuild)
        {
            sensor.AddObservation(endOfBridgeArray[ile].transform.position);
            etat = 2;
        }

        if (etat == 2)
        {
            Vector3 actualPos = gameObject.transform.position;

            float distPrev = Vector3.Distance(prevPos, endOfBridgeArray[ile].transform.position);
            float distNow = Vector3.Distance(actualPos, endOfBridgeArray[ile].transform.position);
            if (distPrev > distNow)
            {
                SetReward(0.05f); // Add a negative reward for the change

            }
            else
            {
                SetReward(-0.05f); // Add a negative reward for the change

            }



        }
        prevPos = gameObject.transform.position;




        if (etat != 2 && bridgeTriggerArray[ile] != null && GeneticAlgorithmArray[ile] != null   ) {

            if (ressource)
            {
                sensor.AddObservation(bridgeTriggerArray[ile].transform.position);
            }


            else
            {
                Vector3[] treePositions = GeneticAlgorithmArray[ile].treeObjects.Select(tree => tree.transform.position).ToArray();
                int[] closestTreeIndices = GetClosestTreeIndices(transform.position, treePositions, 7 - vision);


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

        


        
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotateY = actions.ContinuousActions[1];

        float moveSpeed = 6f * (2f + (float)speed);
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

        if (etat == 1 && ressource)
        {
            if (other.gameObject == bridgeTriggerArray[ile])
            {
                SetReward(4f);
                Debug.Log("bridge");
                ressource = false;
                etat = 0;

            }
        }


        if (etat == 0 && !ressource)
        {
            if (other.CompareTag("tree"))
            {
                SetReward(2f);
                Debug.Log("tree");
                if (ressource == false)
                {
                    LSystemTree lSystemTree = other.GetComponent<LSystemTree>();
                    if (lSystemTree.currentHealthPoint == 1 && GeneticAlgorithmArray[ile].treeObjects != null) GeneticAlgorithmArray[ile].treeObjects.Remove(other.gameObject);
                    if (lSystemTree != null) lSystemTree.LooseHealthPoint();
                }
                ressource = true;
                etat = 1;
            }
        }


        if (other.gameObject == endOfBridgeArray[ile])
        {
            SetReward(2f);
            Debug.Log("endOfBridge");
            ile++;
            ile = ile % 4;
            etat = 0;
            ressource = false;
        }





        if (other.CompareTag("water"))
        {
            SetReward(-2f);
            Debug.Log("water");
            //Destroy(gameObject);
            mort = true;
            EndEpisode();
        }
    }






    private void Awake()
    {
        // Store the initial positions of the agent and targets.
        //initialAgentPosition = transform.position;

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


}
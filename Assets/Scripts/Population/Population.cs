using UnityEngine;

public class Population : MonoBehaviour
{
    public bool addIndividual = false;
    public int populationSize = 3;

    public PopulationGeneticAlgorithm.FitnessAlgorithm fitnessAlgorithm = PopulationGeneticAlgorithm.FitnessAlgorithm.roulette_wheel;

    public double bitMutationRate = 0.1;
    public double swapMutationRate = 0.01;
    public double inversionMutationRate = 0.001;

    public bool strength = false;
    public bool speed = false;
    public bool health = false;
    public bool vision = false;
    public bool smart = false;
    public bool resistance = false;

    private CharacterGenerator CharacterGeneratorScript; 
    private PopulationGeneticAlgorithm PopulationGeneticAlgorithmScript; 

    public enum GenomeInformations
    {
        eyeSize1 = 0,
        eyeSize2 = 1,
        eyeNumber1 = 2,
        eyeNumber2 = 3,
        headShape1 = 4,
        headShape2 = 5,
        headDeformByY = 6,
        headDeformByZ = 7,
        chestShape1 = 8,
        chestShape2 = 9,
        chestDeformByY = 10,
        chestDeformByZ = 11,
        armSize1 = 11,
        armSize2 = 12,
        armNumber1 = 13,
        armNumber2 = 14,
        legSize1 = 16,
        legSize2 = 17,
        legNumber1 = 18,
        legNumber2 = 19,
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Create population");
        CharacterGeneratorScript = GetComponentInChildren<CharacterGenerator>();
        PopulationGeneticAlgorithmScript = GetComponent<PopulationGeneticAlgorithm>();
        if (CharacterGeneratorScript != null)
        {
            for (int i = 0; i < populationSize; i++){
                Character.Individual individual = new Character.Individual();
                individual.SetId(i);
                Character.Capacities wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
                individual.EvaluateFitnessScore(wantedProperties);
                PopulationGeneticAlgorithmScript.AddIndividual(individual);
                CharacterGeneratorScript.GenerateCharacter(i, individual);
                Debug.Log("Information de l'individu "+ i);
                individual.DebugIndividual();
                var navMeshAgentController = CharacterGeneratorScript.GetComponent<NavMeshAgentController>();
                if (navMeshAgentController != null)
                {
                    Vector3 destination = new Vector3(300, 26, 33 + i * 3.5f);
                    navMeshAgentController.SetDestination(destination);
                }
            }
        }
        Debug.Log("Population created");
    }

    private void Update()
    {
        Character.Capacities wantedProperties = new Character.Capacities( vision, smart, resistance, strength, speed);
        Character.MutationRate mutationRate = new Character.MutationRate(bitMutationRate, swapMutationRate, inversionMutationRate);

        foreach (Character.Individual individual in PopulationGeneticAlgorithmScript.individualsSorted){
            individual.EvaluateFitnessScore(wantedProperties);
            individual.UpdateRemainingLife();
            if (individual.isDead())
            {
                CharacterGeneratorScript.DestroyCharacter(individual.GetId());
                Debug.Log("Individual" + individual.GetId() + " died");
                // Retirer personnage de la liste triée
                // Créer id pour décoréler l'id et la populatioSize 
            }
        }
        if (addIndividual)
        {
            Debug.Log("Add Individual");
            populationSize++;
            Character.Individual individual = PopulationGeneticAlgorithmScript.NewGeneration(populationSize, fitnessAlgorithm, wantedProperties, mutationRate);
            PopulationGeneticAlgorithmScript.AddIndividual(individual);
            CharacterGeneratorScript.GenerateCharacter(populationSize, individual);
                
            addIndividual = false;
            Debug.Log("individual added");
        }
    }
}

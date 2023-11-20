using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public float makeABabyProbability = 0.5f;
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

    public GameObject characterPrefab;

    private Character CharacterScript; 
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
        CharacterScript = GetComponentInChildren<Character>();
        CharacterGeneratorScript = GetComponentInChildren<CharacterGenerator>();
        PopulationGeneticAlgorithmScript = GetComponent<PopulationGeneticAlgorithm>();
        if (CharacterGeneratorScript != null && CharacterScript != null)
        {
            for (int i = 0; i < populationSize; i++)
            {
                Character.Individual individual = new Character.Individual();
                individual.SetId(i);
                Debug.Log("Individual id : " + individual.GetId());
                Character.Capacities wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
                individual.EvaluateFitnessScore(wantedProperties);
                PopulationGeneticAlgorithmScript.AddIndividual(individual);
                CharacterGeneratorScript.GenerateCharacter(characterPrefab, i, individual);
                Debug.Log("Information de l'individu "+ i);
                individual.DebugIndividual();
            }
        }
        Debug.Log("Population created");
    }

    private void Update()
    {
        Character.Capacities wantedProperties = new Character.Capacities( vision, smart, resistance, strength, speed);
        Character.MutationRate mutationRate = new Character.MutationRate(bitMutationRate, swapMutationRate, inversionMutationRate);

        Character[] characters = GameObject.FindObjectsOfType<Character>();

        foreach (Character character in characters)
        {
            Character.Individual individual = character.GetIndividual();
            individual.EvaluateFitnessScore(wantedProperties);
            individual.UpdateAge();
            character.SetIndividual(individual);
        }

        foreach (Character character in characters)
        {
            Character.Individual parent1 = character.GetIndividual();
            if (
                parent1 != null &&
                parent1.IsFertile() &&
                MakeABabyProbability() &&
                parent1.IsCoolDownEnded()
                )
            {
                foreach (Character otherCharacter in characters)
                {
                    Character.Individual parent2 = otherCharacter.GetIndividual();
                    if (
                        otherCharacter != character &&
                        parent2.IsFertile() &&
                        parent2.IsCoolDownEnded()
                        )
                    {
                        float distance = Vector3.Distance(character.transform.position, otherCharacter.transform.position);
                        if (distance < Character.proximityDistance)
                        {
                            Debug.Log("Add Individual");
                            Character.Individual child = PopulationGeneticAlgorithmScript.Crossover(parent1, parent2, populationSize, wantedProperties, mutationRate);
                            child.SetId(populationSize);
                            populationSize++;
                            PopulationGeneticAlgorithmScript.AddIndividual(child);
                            CharacterGeneratorScript.GenerateCharacter(characterPrefab, populationSize, child);
                            Debug.Log("Individual added");
                            parent1.TriggerCoolDown();
                            parent2.TriggerCoolDown();
                            otherCharacter.SetIndividual(parent2);
                        }
                    }
                }
            }
            character.SetIndividual(parent1);
        }
    }

    // Vérifie si des individus peuvent faire des enfants
    public bool CanAddIndividual()
    {
        return PopulationGeneticAlgorithmScript.fertileIndividualsSortedByFitnessScore.Count >= 2;
    }

    public bool MakeABabyProbability()
    {
        float proba = UnityEngine.Random.Range(0, 100) / 100.0f;
        return proba <= makeABabyProbability;
    }
}

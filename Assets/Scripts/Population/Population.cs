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
        if (CharacterGeneratorScript != null && PopulationGeneticAlgorithmScript != null)
        {
            var wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
            CreateInitialPopulation(populationSize, characterPrefab, wantedProperties);
        }
        Debug.Log("Population created");
    }

    void CreateInitialPopulation(int size, GameObject prefab, Character.Capacities properties)
    {
        for (int i = 0; i < size; i++)
        {
            CreateIndividual(i, prefab, properties);
        }
    }

    void CreateIndividual(int i, GameObject prefab, Character.Capacities properties)
    {
        Character.Individual individual = new Character.Individual();
        individual.EvaluateFitnessScore(properties);
        PopulationGeneticAlgorithmScript.AddIndividual(individual);
        string name = "Individual" + i;
        CharacterGeneratorScript.GenerateCharacter(individual, prefab, name);

        // Debug
        Debug.Log("Information de l'individu " + i + " :");
        individual.DebugIndividual();
    }

    private void Update()
    {
        Character.Capacities wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
        Character.MutationRate mutationRate = new Character.MutationRate(bitMutationRate, swapMutationRate, inversionMutationRate);

        Character[] characters = GameObject.FindObjectsOfType<Character>();

        UpdatePopulation(characters, wantedProperties);

        if (MakeABabyProbability())
        {    
            TryToReproduceIndividual(characters, wantedProperties, mutationRate);    
        }
    }

    void TryToReproduceIndividual(Character[] characters, Character.Capacities properties, Character.MutationRate mutation)
    {
        foreach (var character in characters)
        {
            Character.Individual parent1 = character.GetIndividual();
            if (IndividualCanMakeABaby(parent1))
            {
                foreach (Character otherCharacter in characters)
                {
                    Character.Individual parent2 = otherCharacter.GetIndividual();
                    if (character != otherCharacter &&
                        IndividualCanMakeABaby(parent2) &&
                        CloseEnough(character, otherCharacter))
                    {
                        MakeABaby(parent1, parent2, properties, mutation);
                        TriggerParentCoolDown(parent1, character);
                        TriggerParentCoolDown(parent2, otherCharacter);
                    }
                }
            }
        }
    }

    void UpdatePopulation(Character[] characters, Character.Capacities properties)
    {
        foreach (var character in characters)
        {
            UpdateIndividual(character, properties);
        }
    }

    void UpdateIndividual(Character character, Character.Capacities properties)
    {
        Character.Individual individual = character.GetIndividual();
        individual.EvaluateFitnessScore(properties);
        individual.UpdateAge();
        character.SetIndividual(individual);
    }

    void TriggerParentCoolDown(Character.Individual parent, Character character)
    {
        parent.TriggerCoolDown();
        character.SetIndividual(parent);
    }

    void MakeABaby(Character.Individual parent1, Character.Individual parent2, Character.Capacities properties, Character.MutationRate mutation)
    {
        Debug.Log("Add Individual");
        Character.Individual child = PopulationGeneticAlgorithmScript.Crossover(parent1, parent2, populationSize, properties, mutation);
        populationSize++;
        PopulationGeneticAlgorithmScript.AddIndividual(child);
        string name = "Individual" + populationSize;
        CharacterGeneratorScript.GenerateCharacter(child, characterPrefab, name);
        Debug.Log("Individual added");
    }

    bool IndividualCanMakeABaby(Character.Individual individual)
    {
        return  individual != null &&
                individual.IsFertile() &&
                individual.IsCoolDownEnded();
    }

    bool MakeABabyProbability()
    {
        float proba = UnityEngine.Random.Range(0, 100) / 100.0f;
        return proba <= makeABabyProbability;
    }

    bool CloseEnough(Character character1, Character character2)
    {
        float distance = Vector3.Distance(character1.transform.position, character2.transform.position);
        return distance < Character.proximityDistance;
    }
}

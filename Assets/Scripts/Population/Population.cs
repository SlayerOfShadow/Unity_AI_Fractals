using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Population : MonoBehaviour
{
    public float makeABabyProbability = 0.5f;
    public int initialPopulationSize = 3;
    private int numberOfIndividuals = 0;

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

    Character.Capacities wantedProperties = new Character.Capacities();

    public int NumberOfIndividuals { get { return numberOfIndividuals; } }
    public GameObject CharacterPrefab { get { return characterPrefab; } }
    public Character.Capacities WantedProperties { get { return wantedProperties; } }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Create population");
        
        CharacterGeneratorScript = GetComponentInChildren<CharacterGenerator>();
        PopulationGeneticAlgorithmScript = GetComponent<PopulationGeneticAlgorithm>();

        if (CharacterGeneratorScript != null && PopulationGeneticAlgorithmScript != null)
        {
            wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
            CreateInitialPopulation(initialPopulationSize, characterPrefab);
        }
        Debug.Log("Population created");
    }

    void CreateInitialPopulation(int initialSize, GameObject prefab)
    {
        for (int i = 0; i < initialSize; i++)
        {
            numberOfIndividuals++;
            CharacterGeneratorScript.GenerateCharacter(new Character.Individual(), prefab, "Individual" + i);
        }
    }

    private void Update()
    {
        wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
        Character.MutationRate mutationRate = new Character.MutationRate(bitMutationRate, swapMutationRate, inversionMutationRate);

        Character[] characters = FindObjectsOfType<Character>();

        if (MakeABabyProbability())
        {    
            TryToReproduceIndividual(characters, wantedProperties, mutationRate, fitnessAlgorithm);    
        }
    }

    List<Character> FertileIndividualsAround(Character[] characters, Character parent1){
        var fertileIndividualsAround = new List<Character>();

        foreach (var character in characters)
        {
            if (parent1 != character &&
                character.CanMakeABaby() &&
                character.CloseEnoughToMakeABaby(parent1))
            {
                fertileIndividualsAround.Add(character);
            }
        }
        return fertileIndividualsAround;
    }

    void TryToReproduceIndividual(Character[] characters, Character.Capacities properties, Character.MutationRate mutation, PopulationGeneticAlgorithm.FitnessAlgorithm algorithm)
    {
        foreach (var character in characters)
        {
            Character.Individual parent1 = character.GetIndividual();
            if (character.CanMakeABaby())
            {
                List<Character> fertileIndividualsAround = FertileIndividualsAround(characters, character);
                if (fertileIndividualsAround.Count > 0)
                {
                    Character parent2 = PopulationGeneticAlgorithmScript.ChooseSecondParent(algorithm, fertileIndividualsAround);
                    MakeABaby(parent1, parent2.GetIndividual(), properties, mutation);
                    character.TriggerCoolDown();
                    parent2.TriggerCoolDown();
                }
            }
        }
    }

    void MakeABaby(Character.Individual parent1, Character.Individual parent2, Character.Capacities properties, Character.MutationRate mutation)
    {
        Debug.Log("Add Individual");
        Character.Individual child = PopulationGeneticAlgorithmScript.Crossover(parent1, parent2, properties, mutation);
        CharacterGeneratorScript.GenerateCharacter(child, characterPrefab, "Individual" + numberOfIndividuals);
        numberOfIndividuals++;
        Debug.Log("Individual added");
    }

    bool MakeABabyProbability()
    {
        float proba = UnityEngine.Random.Range(0, 100) / 100.0f;
        return proba <= makeABabyProbability;
    }
}

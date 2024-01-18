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
       // Debug.Log("Create population");
        
        CharacterGeneratorScript = GetComponentInChildren<CharacterGenerator>();
        PopulationGeneticAlgorithmScript = GetComponent<PopulationGeneticAlgorithm>();

        if (CharacterGeneratorScript != null && PopulationGeneticAlgorithmScript != null)
        {
            wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
            CreateInitialPopulation(initialPopulationSize, characterPrefab);
        }
        DisableModel();
      //  Debug.Log("Population created");
    }

    void CreateInitialPopulation(int initialSize, GameObject characterPrefab)
    {
        for (int i = 0; i < initialSize; i++)
        {
            numberOfIndividuals++;
            CharacterGeneratorScript.GenerateCharacter(new Character.Individual(), characterPrefab, characterPrefab.transform.position, "Individual" + i);
        }
    }

    private void Update()
    {
        wantedProperties = new Character.Capacities(vision, smart, resistance, strength, speed);
        Character.MutationRate mutationRate = new Character.MutationRate(bitMutationRate, swapMutationRate, inversionMutationRate);

        if (IsThereProbabbilityToMakeABaby())
        {    
            Character[] characters = FindObjectsOfType<Character>();
            TryToReproduceIndividuals(characters, wantedProperties, mutationRate, fitnessAlgorithm);    
        }
    }

    // Retourne la liste des individus fertiles suffisament proche du parent choisi pour se reproduire
    // On s'assure aussi que le parent choisi ne se retrouve pas dans la liste
    List<Character> FertileIndividualsAround(Character[] characters, Character parentChosen){
        var fertileIndividualsAround = new List<Character>();
        foreach (var character in characters)
        {
            if (parentChosen != character &&
                character.CanMakeABaby() &&
                character.CloseEnoughToMakeABaby(parentChosen))
            {
                fertileIndividualsAround.Add(character);
            }
        }
        return fertileIndividualsAround;
    }

    void TryToReproduceIndividuals(Character[] characters, Character.Capacities properties, Character.MutationRate mutation, PopulationGeneticAlgorithm.FitnessAlgorithm algorithm)
    {
        foreach (var character in characters)
        {
            if (character.CanMakeABaby())
            {
                List<Character> fertileIndividualsAround = FertileIndividualsAround(characters, character);
                if (fertileIndividualsAround.Count > 0)
                {
                    MakeABaby(
                        character,
                        PopulationGeneticAlgorithmScript.ChooseSecondParent(algorithm, fertileIndividualsAround),
                        properties,
                        mutation);
                }
            }
        }
    }

    void MakeABaby(Character parent1, Character parent2, Character.Capacities properties, Character.MutationRate mutation)
    {
        EnableModel();
        Character.Individual child = PopulationGeneticAlgorithmScript.Crossover(parent1.GetIndividual(), parent2.GetIndividual(), properties, mutation);
        var childPosition = new Vector3(
                    (parent1.transform.position.x + parent2.transform.position.x) / 2f,
                    parent1.transform.position.y,
                    (parent1.transform.position.z + parent2.transform.position.z) / 2f);
        CharacterGeneratorScript.GenerateCharacter(child, characterPrefab, childPosition, "Individual" + numberOfIndividuals);
        parent1.HaveMadeABaby();
        parent2.HaveMadeABaby();
        numberOfIndividuals++;
        DisableModel();
        Debug.Log("Baby made");
    }

    bool IsThereProbabbilityToMakeABaby()
    {
        float proba = UnityEngine.Random.Range(0, 100) / 100.0f;
        return proba <= makeABabyProbability;
    }

    void DisableModel()
    {
        // Désactiver le CharacterModel et le Lorenz (les rendre inactifs)
        if (characterPrefab != null)
        {
            characterPrefab.SetActive(false);
        }
    }

    void EnableModel()
    {
        // Activer le CharacterModel et le Lorenz
        if (characterPrefab != null)
        {
            characterPrefab.SetActive(true);
        }
    }
}

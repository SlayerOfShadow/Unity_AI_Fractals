using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public bool AddIndividual = false;
    public int populationSize = 6;
    private CharacterGenerator CharacterGeneratorScript; 
    private PopulationGeneticAlgorithm PopulationGeneticAlgorithmScript; 
    private Character CharacterScript; 

    public PopulationGeneticAlgorithm.FitnessAlgorithm fitness_algorithm = PopulationGeneticAlgorithm.FitnessAlgorithm.roulette_wheel;

    public double bit_mutation_rate = 0.1;
    public double swap_mutation_rate = 0.01;
    public double inversion_mutation_rate = 0.001;

    public bool strength = false;
    public bool speed = false;
    public bool health = false;
    public bool vision = false;
    public bool smart = false;
    public bool resistance = false;


    // Start is called before the first frame update
    void Start()
    {
        CharacterGeneratorScript = GetComponentInChildren<CharacterGenerator>();
        CharacterScript = GetComponentInChildren<Character>();
        PopulationGeneticAlgorithmScript = GetComponent<PopulationGeneticAlgorithm>();
        if (CharacterGeneratorScript != null)
        {
            for (int i = 0; i < populationSize; i++){
                Character.Individual individual = new Character.Individual();
                individual.GenerateGenome();
                Character.Properties wanted_properties = new Character.Properties(strength, speed, health, vision, smart, resistance);
                individual.evaluate_fitness_score(wanted_properties);
                PopulationGeneticAlgorithmScript.add_individual(individual);
                CharacterGeneratorScript.GenerateCharacter(i, individual);
                var navMeshAgentController = CharacterGeneratorScript.GetComponent<NavMeshAgentController>();
                if (navMeshAgentController != null)
                {
                    Vector3 destination = new Vector3(300, 26, 33 + i*3.5f);
                    navMeshAgentController.SetDestination(destination);
                }
            }
        }
    }

    private void Update()
    {
        if (AddIndividual){
            Character.Properties wanted_properties = new Character.Properties(strength, speed, health, vision, smart, resistance);
            Character.MutationRate mutation_rate = new Character.MutationRate(bit_mutation_rate, swap_mutation_rate, inversion_mutation_rate);

            foreach (Character.Individual individual in PopulationGeneticAlgorithmScript.individualsSorted){
                individual.evaluate_fitness_score(wanted_properties);
            }
            PopulationGeneticAlgorithmScript.new_generation(populationSize, CharacterScript.genome_size, fitness_algorithm, wanted_properties, mutation_rate);
            AddIndividual = false;
        }
    }
    // Update is called once per frame

    void AddNewIndividual(){
        populationSize++;
        Character.Individual individual = new Character.Individual();
        individual.GenerateGenome();
        // attention si on change les properties il faut réévaluer tous les fitness score
        Character.Properties wanted_properties = new Character.Properties(strength, speed, health, vision, smart, resistance);
        Character.MutationRate mutation_rate = new Character.MutationRate(bit_mutation_rate, swap_mutation_rate, inversion_mutation_rate);
        individual.evaluate_fitness_score(wanted_properties);
        PopulationGeneticAlgorithmScript.add_individual(individual);
        CharacterGeneratorScript.GenerateCharacter(populationSize, individual);
        var navMeshAgentController = CharacterGeneratorScript.GetComponent<NavMeshAgentController>();
        if (navMeshAgentController != null)
        {
            Vector3 destination = new Vector3(300,26,33);
            navMeshAgentController.SetDestination(destination);
        }
    }


}

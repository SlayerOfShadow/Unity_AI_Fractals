using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int populationSize = 50;
    [SerializeField] float mutationRate = 0.1f;
    [SerializeField] int maxGenerations = 100;
    [SerializeField] float simulationSpeed = 0.5f;
    [SerializeField] float maxTreeHeight = 5;
    [SerializeField] bool tournament = true;
    [SerializeField] int tournamentSize = 5;
    List<Tree> population = new List<Tree>();
    GameObject[] treeObjects;
    Terrain terrain;
    Vector3 terrainSize;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainSize = terrain.terrainData.size;

        treeObjects = new GameObject[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            treeObjects[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        }

        InitializePopulation();

        StartCoroutine(RunGeneticAlgorithm());
        GeneratePopulation();
    }

    IEnumerator RunGeneticAlgorithm()
    {
        int generation = 0;
        while (generation < maxGenerations)
        {
            // Evaluate fitness
            EvaluateFitness();

            // Check if all trees are below y = 1
            bool allTreesBelowThreshold = true;
            foreach (Tree tree in population)
            {
                if (tree.Position.y > maxTreeHeight)
                {
                    allTreesBelowThreshold = false;
                    break; // Exit the loop if any tree is not below y = 1
                }
            }

            if (allTreesBelowThreshold)
            {
                Debug.Log("All trees are below y = 1. Stopping generation.");
                break; // Stop the algorithm if all trees are below y = 1
            }

            // Selection, Crossover, and Mutation
            List<Tree> newPopulation = new List<Tree>();
            Tree parent1;
            Tree parent2;
            for (int i = 0; i < populationSize; i++)
            {
                if (tournament)
                {
                    parent1 = SelectParentTournament();
                    parent2 = SelectParentTournament();
                } else
                {
                    parent1 = SelectParentRoulette();
                    parent2 = SelectParentRoulette();
                }
                
                Tree child = Crossover(parent1, parent2);
                Mutate(child);
                newPopulation.Add(child);
            }

            // Replace the old population
            population = newPopulation;

            // Generate trees
            GeneratePopulation();

            generation++;
            yield return new WaitForSeconds(simulationSpeed);
        }
    }

    void InitializePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            // Generate random position within the terrain bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(0, terrainSize.x),
                0,
                Random.Range(0, terrainSize.z)
            );

            // Adjust the Y position based on terrain height
            randomPosition.y = terrain.SampleHeight(randomPosition);

            Tree tree = new Tree(randomPosition);
            population.Add(tree);
        }
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < population.Count; i++)
        {
            treeObjects[i].transform.position = population[i].Position;
        }
    }

    void EvaluateFitness()
    {
        foreach (Tree tree in population)
        {
            tree.Fitness = -tree.Position.y;
        }
    }

    // TOURNAMENT
    Tree SelectParentTournament()
    {
        // Tournament size defines how many individuals will compete to become a parent
        // Randomly select 'tournamentSize' individuals from the population
        List<Tree> tournamentParticipants = new List<Tree>();
        for (int i = 0; i < tournamentSize; i++)
        {
            int randomIndex = Random.Range(0, population.Count);
            tournamentParticipants.Add(population[randomIndex]);
        }

        // Find the individual with the lowest fitness in the tournament
        Tree bestParent = tournamentParticipants[0];
        float bestFitness = bestParent.Fitness;

        foreach (Tree participant in tournamentParticipants)
        {
            if (participant.Fitness > bestFitness)
            {
                bestParent = participant;
                bestFitness = participant.Fitness;
            }
        }

        return bestParent;
    }

    // ROULETTE WHEEL
    Tree SelectParentRoulette()
    {
        // Calculate the total fitness of the population
        float totalFitness = 0f;
        foreach (Tree tree in population)
        {
            totalFitness += tree.Fitness;
        }

        // Generate a random value between 0 and the total fitness
        float randomValue = Random.Range(0f, totalFitness);

        // Iterate through the population and find the individual that corresponds to the random value
        float cumulativeFitness = 0f;
        foreach (Tree tree in population)
        {
            cumulativeFitness += tree.Fitness;
            if (cumulativeFitness >= randomValue)
            {
                return tree; // This tree is selected as a parent
            }
        }

        // In case of rounding errors or extreme fitness values, return the last individual
        return population[population.Count - 1];
    }


    Tree Crossover(Tree parent1, Tree parent2)
    {
        // Implement crossover method (e.g., uniform crossover or single-point crossover)
        // Here, we'll use single-point crossover.
        Vector3 childPosition = Vector3.zero;

        if (Random.Range(0f, 1f) < 0.5f)
        {
            childPosition.x = parent1.Position.x;
            childPosition.z = parent2.Position.z;
        }
        else
        {
            childPosition.x = parent2.Position.x;
            childPosition.z = parent1.Position.z;
        }

        childPosition.y = terrain.SampleHeight(childPosition);

        return new Tree(childPosition);
    }

    void Mutate(Tree tree)
    {
        // Implement mutation method
        if (Random.Range(0f, 1f) < mutationRate)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(0, terrainSize.x),
                0,
                Random.Range(0, terrainSize.z)
            );

            // Adjust the Y position based on terrain height
            randomPosition.y = terrain.SampleHeight(randomPosition);
            tree.Position = randomPosition;
        }
    }

    private class Tree
    {
        public Vector3 Position { get; set; }
        public float Fitness { get; set; }

        public Tree(Vector3 position)
        {
            Position = position;
        }
    }
}

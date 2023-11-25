using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int populationSize = 50;
    [SerializeField] float mutationRate = 0.1f;
    [SerializeField] int maxGenerations = 100;
    [SerializeField] float maxTreeHeight = 5;
    [SerializeField] bool tournament = true;
    [SerializeField] int tournamentSize = 5;
    [SerializeField] float waterHeight = 10;
    List<Tree> population = new List<Tree>();
    public GameObject[] treeObjects;
    Terrain terrain;
    Vector3 terrainSize;
    [SerializeField] Vector2 randTreeScale = new Vector2(3, 5);

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainSize = terrain.terrainData.size;

        treeObjects = new GameObject[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            treeObjects[i] = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            treeObjects[i].transform.localScale = Vector3.one * Random.Range(randTreeScale.x, randTreeScale.y);
        }

        InitializePopulation();

        RunGeneticAlgorithm();
    }

    void RunGeneticAlgorithm()
    {
        int generation = 0;
        while (generation < maxGenerations)
        {
            // Evaluate fitness
            EvaluateFitness();

            // Check if all trees have good position
            bool allTreesBelowThreshold = true;
            foreach (Tree tree in population)
            {
                if (tree.Position.y > maxTreeHeight || tree.Position.y < waterHeight)
                {
                    Tree parent1;
                    Tree parent2;
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
                    tree.Position = child.Position;
                    tree.Fitness = child.Fitness;
                    allTreesBelowThreshold = false;
                }
            }

            if (allTreesBelowThreshold)
            {
                Debug.Log("All trees are below y = " + maxTreeHeight + " and above y = " + waterHeight + ". Stopping generation. Count = " + generation + ".");
                break; 
            }

            // Generate trees
            GeneratePopulation();

            generation++;
        }
    }

    void InitializePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            // Generate random position within the terrain bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(transform.position.x, transform.position.x + terrainSize.x),
                0,
                Random.Range(transform.position.z, transform.position.z + terrainSize.z)
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
        float bestFitness = (maxTreeHeight + waterHeight) * 0.5f;
        foreach (Tree tree in population)
        {
            float distanceToBestFitness = Mathf.Abs(tree.Position.y - bestFitness);
            tree.Fitness = -distanceToBestFitness;
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
                Random.Range(transform.position.x, transform.position.x + terrainSize.x),
                0,
                Random.Range(transform.position.z, transform.position.z + terrainSize.z)
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

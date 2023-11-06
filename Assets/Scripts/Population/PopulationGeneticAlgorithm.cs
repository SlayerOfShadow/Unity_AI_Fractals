using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

public class PopulationGeneticAlgorithm : MonoBehaviour
{
    private Character CharacterScript; 

    public enum FitnessAlgorithm
    {
        random,
        roulette_wheel,
    }

    public List<Character.Individual> individualsSorted = new List<Character.Individual>();

    private void Sort()
    {
        individualsSorted.Sort((individu1, individu2) => individu2.fitnessScore.CompareTo(individu1.fitnessScore));
    }

    public void add_individual(Character.Individual individual)
    {
        individualsSorted.Add(individual);
        Sort();
    }

    public void pop_individual()
    {
        individualsSorted.Remove(individualsSorted.Last());
    }

    public Character.Individual crossover(Character.Individual parent1, Character.Individual parent2, int population_length, Character.Capacities properties, Character.MutationRate mutation_rate)
    {            
        // Créer deux enfants en copiant les gènes des parents
        Character.Genome genome1 = new Character.Genome();
        Character.Genome genome2 = new Character.Genome();

        int genomeSize = genome1.Size();

        for (int i = 0; i < genomeSize; i++)
        {
            genome1.SetByIndex(i,UnityEngine.Random.Range(0, 2)); 
            genome2.SetByIndex(i,UnityEngine.Random.Range(0, 2));
        }

        // Crossover
        int crossover = UnityEngine.Random.Range(0, genomeSize); // Point de croisement aléatoire
        Character.Individual child1 = new Character.Individual();
        Character.Individual child2 = new Character.Individual();
        
        child1.genome.Set(genome1);
        child2.genome.Set(genome2);

        for (int i = 0; i < crossover; i++)
        {
            child1.genome.SetByIndex(i, parent1.genome.GetIndex(i));
            child2.genome.SetByIndex(i, parent2.genome.GetIndex(i));
        }

        for (int i = crossover; i < parent1.genome.Get().Length; i++)
        {
            child1.genome.SetByIndex(i, parent2.genome.GetIndex(i));
            child2.genome.SetByIndex(i, parent1.genome.GetIndex(i));
        }

        // mutation
        child1.mutation(mutation_rate);
        child2.mutation(mutation_rate);

        // Réévaluer les enfants
        child1.evaluate_fitness_score(properties);
        child2.evaluate_fitness_score(properties);

        // evolution 
        if (child1.fitnessScore >= child2.fitnessScore)
        {
            return evolve(child1, population_length);
        }
        else
        {
            return evolve(child2, population_length);
        }
    }

    public Character.Individual evolve(Character.Individual child, int population_length)
    {
        add_individual(child);
        child.evaluate_statistics();
        return child;
    }

    public void choose_parent(out Character.Individual parent1, out Character.Individual parent2, FitnessAlgorithm algorithm)
    {
        int index_parent1 = -1;
        int index_parent2 = -1;
        switch (algorithm)
        {
            case FitnessAlgorithm.random:
                random_selection(out index_parent1, out index_parent2);
                break;
            case FitnessAlgorithm.roulette_wheel:
                roulette_wheel_selection(out index_parent1, out index_parent2);
                break;
            default:
                Debug.LogError("Algorithme non pris en charge.");
                break;
        }
        parent1 = individualsSorted[index_parent1];
        parent2 = individualsSorted[index_parent2];
    }

    public Character.Individual new_generation(int population_length, FitnessAlgorithm algorithm, Character.Capacities properties, Character.MutationRate mutation_rate)
    {
        Character.Individual parent1, parent2;
        choose_parent(out parent1, out parent2, algorithm);
        return crossover(parent1, parent2, population_length, properties, mutation_rate);
    }

    public void roulette_wheel_selection(out int index_parent1, out int index_parent2)
    {
        // Calculer la somme des scores de fitness de tous les individus
        int totalFitness = individualsSorted.Sum(individual => individual.fitnessScore);

        // Générer un nombre aléatoire entre 0 et la somme des scores de fitness
        int random_number_1 = UnityEngine.Random.Range(0, totalFitness);

        // Sélectionner le premier parent
        index_parent1 = select_index_by_roulette(random_number_1);

        // Calculer la somme des scores de fitness des individus restants (en excluant le premier parent)
        int total_remaining_fitness = totalFitness - individualsSorted[index_parent1].fitnessScore;

        // Générer un nombre aléatoire pour sélectionner le deuxième parent parmi les individus restants
        int random_number_2 = UnityEngine.Random.Range(0, total_remaining_fitness);

        // Sélectionner le deuxième parent
        index_parent2 = select_index_by_roulette(random_number_2, index_parent1);
    }

    // Fonction de sélection par roulette pour obtenir un indice
    private int select_index_by_roulette(int random_number, int excluded_index = -1)
    {
        int accumulated_fitness = 0;
        for (int i = 0; i < individualsSorted.Count; i++)
        {
            if (i != excluded_index) // Exclure l'indice spécifié (si fourni)
            {
                accumulated_fitness += individualsSorted[i].fitnessScore;
                if (accumulated_fitness >= random_number)
                {
                    return i; // Retourner l'indice sélectionné
                }
            }
        }
        // En cas d'échec
        Debug.Log("La sélection par roulette a échoué.");
        return -1;
    }

    public void random_selection(out int index_parent1, out int index_parent2)
    {
        index_parent1 = UnityEngine.Random.Range(0, individualsSorted.Count - 1);
        index_parent2 = UnityEngine.Random.Range(0, individualsSorted.Count - 1);
        while (index_parent1 == index_parent2)
        {
            index_parent2 = UnityEngine.Random.Range(0, individualsSorted.Count - 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

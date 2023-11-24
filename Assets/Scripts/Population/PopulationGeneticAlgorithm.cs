using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationGeneticAlgorithm : MonoBehaviour
{
    public enum FitnessAlgorithm
    {
        random,
        roulette_wheel,
    }

    public Character.Individual Crossover(Character.Individual parent1, Character.Individual parent2, Character.Capacities properties, Character.MutationRate mutationRate)
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
        Character.Individual child1 = new Character.Individual(genome1);
        Character.Individual child2 = new Character.Individual(genome2);

        for (int i = 0; i < crossover; i++)
        {
            child1.SetGenomeByIndex(i, parent1.GetGenomeByIndex(i));
            child2.SetGenomeByIndex(i, parent2.GetGenomeByIndex(i));
        }

        for (int i = crossover; i < genomeSize; i++)
        {
            child1.SetGenomeByIndex(i, parent2.GetGenomeByIndex(i));
            child2.SetGenomeByIndex(i, parent1.GetGenomeByIndex(i));
        }

        // mutation
        child1.Mutation(mutationRate);
        child2.Mutation(mutationRate);

        // Réévaluer les enfants
        child1.EvaluateFitnessScore(properties);
        child2.EvaluateFitnessScore(properties);

        // evolution 
        if (child1.GetFitnessScore() >= child2.GetFitnessScore())
        {
            return child1;
        }
        else
        {
            return child2;
        }
    }

    private int RouletteWheelSelection(List<Character> fertileIndividualsAround)
    {
        // trier la liste en fonction des scores de fitness
        fertileIndividualsAround.Sort((individu1, individu2) => individu2.GetIndividual().GetFitnessScore().CompareTo(individu1.GetIndividual().GetFitnessScore()));

        // Calculer la somme des scores de fitness de tous les individus
        int totalFitness = fertileIndividualsAround.Sum(individual => individual.GetIndividual().GetFitnessScore());

        // Générer un nombre aléatoire entre 0 et la somme des scores de fitness
        int randomNumber = UnityEngine.Random.Range(0, totalFitness);

        return SelectIndexByRoulette(randomNumber, fertileIndividualsAround);
    }

    // Fonction de sélection par roulette pour obtenir un indice
    private int SelectIndexByRoulette(int randomNumber, List<Character> fertileIndividualsAround)
    {
        int accumulatedFitness = 0;
        for (int i = 0; i < fertileIndividualsAround.Count; i++)
        {
            accumulatedFitness += fertileIndividualsAround[i].GetIndividual().GetFitnessScore();
            if (accumulatedFitness >= randomNumber)
            {
                return i;
            }
        }
        // En cas d'échec
        Debug.Log("La sélection par roulette a échoué.");
        return -1;
    }

    private int RandomSelection(int listSize)
    {
        return UnityEngine.Random.Range(0, listSize - 1);
    }

    public Character ChooseSecondParent(FitnessAlgorithm algorithm, List<Character> fertileIndividualsAround)
    {
        switch (algorithm)
        {
            case FitnessAlgorithm.random:
                return fertileIndividualsAround[RandomSelection(fertileIndividualsAround.Count)];
            case FitnessAlgorithm.roulette_wheel:
                return fertileIndividualsAround[RouletteWheelSelection(fertileIndividualsAround)];
            default:
                Debug.LogError("Algorithme non pris en charge. L'algorithme par défaut de la séléction aléatoire est appliqué.");
                return fertileIndividualsAround[RandomSelection(fertileIndividualsAround.Count)];
        }
    }
}
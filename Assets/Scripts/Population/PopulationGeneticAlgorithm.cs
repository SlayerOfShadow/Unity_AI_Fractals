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

    public List<Character.Individual> fertileIndividualsSortedByFitnessScore = new List<Character.Individual>();

    private void Sort()
    {
        fertileIndividualsSortedByFitnessScore.Sort((individu1, individu2) => individu2.GetFitnessScore().CompareTo(individu1.GetFitnessScore()));
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

    public void ChooseParent(out Character.Individual parent1, out Character.Individual parent2, FitnessAlgorithm algorithm)
    {
        int indexParent1 = -1;
        int indexParent2 = -1;
        switch (algorithm)
        {
            case FitnessAlgorithm.random:
                RandomSelection(out indexParent1, out indexParent2);
                break;
            case FitnessAlgorithm.roulette_wheel:
                RouletteWheelSelection(out indexParent1, out indexParent2);
                break;
            default:
                Debug.LogError("Algorithme non pris en charge.");
                break;
        }
        parent1 = fertileIndividualsSortedByFitnessScore[indexParent1];
        parent2 = fertileIndividualsSortedByFitnessScore[indexParent2];
    }

    public Character.Individual NewGeneration(FitnessAlgorithm algorithm, Character.Capacities properties, Character.MutationRate mutationRate)
    {
        Character.Individual parent1, parent2;
        ChooseParent(out parent1, out parent2, algorithm);
        return Crossover(parent1, parent2, properties, mutationRate);
    }

    private void RouletteWheelSelection(out int indexParent1, out int indexParent2)
    {
        // Calculer la somme des scores de fitness de tous les individus
        int totalFitness = fertileIndividualsSortedByFitnessScore.Sum(individual => individual.GetFitnessScore());

        // Générer un nombre aléatoire entre 0 et la somme des scores de fitness
        int randomNumber1 = UnityEngine.Random.Range(0, totalFitness);

        // Sélectionner le premier parent
        indexParent1 = SelectIndexByRoulette(randomNumber1);

        // Calculer la somme des scores de fitness des individus restants (en excluant le premier parent)
        int totalRemainingFitness = totalFitness - fertileIndividualsSortedByFitnessScore[indexParent1].GetFitnessScore();

        // Générer un nombre aléatoire pour sélectionner le deuxième parent parmi les individus restants
        int randomNumber2 = UnityEngine.Random.Range(0, totalRemainingFitness);

        // Sélectionner le deuxième parent
        indexParent2 = SelectIndexByRoulette(randomNumber2, indexParent1);
    }

    // Fonction de sélection par roulette pour obtenir un indice
    private int SelectIndexByRoulette(int randomNumber, int excludedIndex = -1)
    {
        int accumulatedFitness = 0;
        for (int i = 0; i < fertileIndividualsSortedByFitnessScore.Count; i++)
        {
            if (i != excludedIndex) // Exclure l'indice spécifié (si fourni)
            {
                accumulatedFitness += fertileIndividualsSortedByFitnessScore[i].GetFitnessScore();
                if (accumulatedFitness >= randomNumber)
                {
                    return i; // Retourner l'indice sélectionné
                }
            }
        }
        // En cas d'échec
        Debug.Log("La sélection par roulette a échoué.");
        return -1;
    }

    private void RandomSelection(out int indexParent1, out int indexParent2)
    {
        indexParent1 = UnityEngine.Random.Range(0, fertileIndividualsSortedByFitnessScore.Count - 1);
        indexParent2 = UnityEngine.Random.Range(0, fertileIndividualsSortedByFitnessScore.Count - 1);
        while (indexParent1 == indexParent2)
        {
            indexParent2 = UnityEngine.Random.Range(0, fertileIndividualsSortedByFitnessScore.Count - 1);
        }
    }

    public Character ChooseSecondParent(PopulationGeneticAlgorithm.FitnessAlgorithm algorithm, List<Character> fertileIndividuals)
    {
        Character parent2 = new Character();
        switch (algorithm)
        {
            case PopulationGeneticAlgorithm.FitnessAlgorithm.random:
                parent2 = fertileIndividuals[UnityEngine.Random.Range(0, fertileIndividuals.Count - 1)];
                break;
            case PopulationGeneticAlgorithm.FitnessAlgorithm.roulette_wheel:
                fertileIndividuals.Sort((individu1, individu2) => individu2.GetIndividual().GetFitnessScore().CompareTo(individu1.GetIndividual().GetFitnessScore()));
                int totalFitness = fertileIndividuals.Sum(individual => individual.GetIndividual().GetFitnessScore());
                int randomNumber = UnityEngine.Random.Range(0, totalFitness);
                int accumulatedFitness = 0;
                int index = 0;
                for (int i = 0; i < fertileIndividuals.Count; i++)
                {
                    accumulatedFitness += fertileIndividuals[i].GetIndividual().GetFitnessScore();
                    if (accumulatedFitness >= randomNumber)
                    {
                        index = i;
                        break;
                    }
                }
                parent2 = fertileIndividuals[index];
                break;
            default:
                Debug.LogError("Algorithme non pris en charge.");
                break;
        }
        return parent2;
    }
}
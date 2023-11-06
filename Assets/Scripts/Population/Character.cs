using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System;

public class Character : MonoBehaviour
{
    public class Genome{
        private int[] _value;
        private int _size = 26;
        public Genome(int[] value){
            _value = value;
        }
        public Genome(){
            _value = new int[_size];
        }
        public Genome(Genome g){
            _value = g.Get();
        }
        public void Set(Genome g){
            for (int i = 0; i < _size; i++)
            {
                _value[i] = g.GetIndex(i);
            }
        }
        public int Size(){
            return _size;
        }
        public int[] Get(){
            return _value;
        }
        public int GetIndex(int i){
            return _value[i];
        }
        public void SetByIndex(int i, int value){
            _value[i] = value;
        }
        public int[] PartialGenome(int start, int number_of_elements){
            return Enumerable.Range(start, number_of_elements).Select(i => _value[i]).ToArray();
        }
    }

    public class Capacities{
        public bool strength;
        public bool speed;
        public bool health;
        public bool vision;
        public bool smart;
        public bool resistance;
        public Capacities(bool _strength, bool _speed, bool _health, bool _vision, bool _smart, bool _resistance){
            strength = _strength;
            speed = _speed;
            health = _health;
            vision = _vision;
            smart = _smart;
            resistance = _resistance;
        }
    }

    public class CapacitiesStatistics{
        public int strength;
        public int speed;
        public int health;
        public int vision;
        public int smart;
        public int resistance;
        public CapacitiesStatistics(){
            strength = 0 ;
            speed = 0 ;
            health = 0 ;
            vision = 0 ;
            smart = 0 ;
            resistance = 0 ;
        }
    }

    public class MutationRate
    {
        public double bit;
        public double swap;
        public double inversion;
        public MutationRate(double bit_mutation, double swap_mutation, double inversion_mutation)
        {
            bit = bit_mutation;
            swap = swap_mutation;
            inversion = inversion_mutation;
        }
    }

    public class Individual{
        public Genome genome;
        public int fitnessScore;
        public int remainingLife = 180; // frame
        public CapacitiesStatistics statistics = new CapacitiesStatistics(); // en fonction des gènes de l'individu il aura des stats de capacités différentes

        public Individual(){
            GenerateGenome();
        }

        public Individual(Genome g){
            genome.Set(g);
        }

        public int GenomeSize(){
            return genome.Size();
        }

        public void UpdateRemainingLife(){
            remainingLife--;
        }
        public int GetRemainingLife(){
            return remainingLife;
        }

        public void evaluate_statistics(){
            statistics.speed = genome.GetIndex(13)+genome.GetIndex(12) + (genome.GetIndex(14)+genome.GetIndex(15)+genome.GetIndex(16)+genome.GetIndex(17))*2;
            statistics.strength = genome.GetIndex(18)+genome.GetIndex(19) + (genome.GetIndex(20)+genome.GetIndex(21))*2;
            statistics.health = genome.GetIndex(22)+genome.GetIndex(23)+genome.GetIndex(24)+genome.GetIndex(25);
            statistics.resistance = genome.GetIndex(9)+genome.GetIndex(8) + (genome.GetIndex(11)+genome.GetIndex(10))*2;
            statistics.smart = genome.GetIndex(4)+genome.GetIndex(5) + (genome.GetIndex(7)+genome.GetIndex(6))*2;
            statistics.vision = genome.GetIndex(0)+genome.GetIndex(1) + (genome.GetIndex(2)+genome.GetIndex(3))*2;
        }

        public void evaluate_fitness_score(Capacities Capacities){
            fitnessScore=0;
            if(Capacities.speed){
                fitnessScore+=genome.GetIndex(13)+genome.GetIndex(12);
                fitnessScore+=(genome.GetIndex(14)+genome.GetIndex(15)+genome.GetIndex(16)+genome.GetIndex(17))*2;
            }
            if(Capacities.strength){
                fitnessScore+=genome.GetIndex(18)+genome.GetIndex(19);
                fitnessScore+=(genome.GetIndex(20)+genome.GetIndex(21))*2;
            }
            if(Capacities.health){
                fitnessScore+=genome.GetIndex(22)+genome.GetIndex(23)+genome.GetIndex(24)+genome.GetIndex(25);
            }
            if(Capacities.resistance){
                fitnessScore+=genome.GetIndex(9)+genome.GetIndex(8);
                fitnessScore+=(genome.GetIndex(11)+genome.GetIndex(10))*2;
            }    
            if(Capacities.smart){
                fitnessScore+=genome.GetIndex(4)+genome.GetIndex(5);
                fitnessScore+=(genome.GetIndex(7)+genome.GetIndex(6))*2;
            }   
            if(Capacities.vision){
                fitnessScore+=genome.GetIndex(0)+genome.GetIndex(1);
                fitnessScore+=(genome.GetIndex(2)+genome.GetIndex(3))*2;
            }     
        }

        /*    
        Mutation non codées :
        - Mutation de valeur : Modifier la valeur d'un gène à un autre aléatoire dans une certaine plage. 
            Par exemple, ajouter ou soustraire une petite valeur à un gène.
            -> intérêt si gène pas que entre 0 et 1
        - Mutation par insertion ou suppression : Ajoutez ou supprimez un gène du génome.
            -> là on a des génome fixe donc peu pratique
        */

        // Mutation par permutation : Permute l'emplacement de deux gènes dans le génome.
        public void swap_mutation(double mutation_rate)
        {
            for (int i = 0; i < GenomeSize(); i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    int index1 = UnityEngine.Random.Range(0, GenomeSize());
                    int index2 = UnityEngine.Random.Range(0, GenomeSize());

                    // Échanger les valeurs des bits à index1 et index2
                    int temp = genome.GetIndex(index1);
                    genome.SetByIndex(index1, genome.GetIndex(index2));
                    genome.SetByIndex(index2, temp);
                }
            }
        }

        //Mutation par inversion : Inverse l'ordre des gènes dans une partie du génome.
        public void inversion_mutation(double mutation_rate)
        {
            for (int i = 0; i < GenomeSize(); i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    int start = UnityEngine.Random.Range(0, GenomeSize());
                    int end = UnityEngine.Random.Range(start, GenomeSize());

                    // Inverse l'ordre des bits entre start et end inclus
                    while (start < end)
                    {
                        int temp = genome.GetIndex(start);
                        genome.SetByIndex(start, genome.GetIndex(end));
                        genome.SetByIndex(end, temp);
                        start++;
                        end--;
                    }
                }
            }
        }

        // Mutation de bit : Inverse le bit(0 devient 1, et vice versa) à un emplacement aléatoire du génome.
        public void bit_mutation(double mutation_rate)
        {
            for (int i = 0; i < GenomeSize(); i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    // Inverser le bit (0 devient 1, 1 devient 0)
                    genome.SetByIndex(i, 1 - genome.GetIndex(i));
                }
            }
        }

        public void GenerateGenome()
        {
            genome = new Genome();
            for (int i = 0; i < genome.Size(); i++)
            {
                genome.SetByIndex(i, UnityEngine.Random.Range(0, 2)); // Remplir le genome avec des 0 ou des 1
            }
            Debug.Log("Le Génome de l'individu est : "+ string.Join(", ", genome.Get()));
        }

        public void mutation(MutationRate mutation_rate)
        {
            bit_mutation(mutation_rate.bit);
            swap_mutation(mutation_rate.swap);
            inversion_mutation(mutation_rate.inversion);
        }

        public void DebugIndividual(){
            Debug.Log("Génome :"+genome);
            Debug.Log("Score de fitness : "+fitnessScore);
            Debug.Log("Espérence de vie : "+remainingLife);
            Debug.Log("Statistiques : ");
            Debug.Log("Force : "+ statistics.strength);
            Debug.Log("Vitesse : "+ statistics.speed);
            Debug.Log("Vision : "+ statistics.vision);
            Debug.Log("Santé : "+ statistics.health);
            Debug.Log("Résistance : "+ statistics.resistance);
            Debug.Log("Intelligence : "+ statistics.smart);
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

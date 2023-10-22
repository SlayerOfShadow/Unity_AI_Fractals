using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor.Rendering;
using UnityEditor;

public class DynamicModelGenerator : MonoBehaviour
{
    public bool make_a_new_generation = false;
    public FitnessAlgorithm fitness_algorithm = FitnessAlgorithm.roulette_wheel;
    public double bit_mutation_rate = 0.1;
    public double swap_mutation_rate = 0.01;
    public double inversion_mutation_rate = 0.001;
    public bool strength = false;
    public bool speed = false;
    public bool health = false;
    public bool vision = false;
    public bool smart = false;
    public bool resistance = false;

    private static int genome_size = 26;
    private static int population_size = 6;
    public GameObject eyePrefab; // The prefab for the eye sphere
    public GameObject headPrefab1; // The prefab for head 1
    public GameObject headPrefab2; // The prefab for head 2
    public GameObject headPrefab3; // The prefab for head 3
    public GameObject headPrefab4; // The prefab for head 4
    public GameObject chestPrefab1; // The prefab for chest 1
    public GameObject chestPrefab2; // The prefab for chest 2
    public GameObject chestPrefab3; // The prefab for chest 3
    public GameObject chestPrefab4; // The prefab for chest 4
    public GameObject legPrefab;
    public GameObject armPrefab;

public GameObject modelObject;

    Population population = new Population();

    void Start()
    {
        for (int j = 0; j < population_size; j++)
        {
        int[] genome = new int[26]; // Déclarez et initialisez le tableau genome
            for (int i = 0; i < genome_size; i++)
            {
                genome[i] = UnityEngine.Random.Range(0, 2); // Remplissez le tableau genome avec des 0 ou des 1
            }

            // Maintenant que le génome est généré, vous pouvez créer une instance de la classe Individual
            Individual individual = new Individual(genome);
            Properties wanted_properties = new Properties(strength,speed,health,vision,smart,resistance);
            individual.evaluate_fitness_score(wanted_properties);
            population.add_individual(individual);

            // Utilisez cet individu comme vous le souhaitez
            string adn_eye = string.Join("", individual.genome.Take(4).Select(bit => bit.ToString()));
            string adn_head = string.Join("", individual.genome.Skip(4).Take(5).Select(bit => bit.ToString()));
            string adn_chest = string.Join("", individual.genome.Skip(8).Take(5).Select(bit => bit.ToString()));
            string adn_legs = string.Join("", individual.genome.Skip(12).Take(7).Select(bit => bit.ToString()));
            string adn_arms = string.Join("", individual.genome.Skip(18).Take(5).Select(bit => bit.ToString()));
            // population.create_population(population_size, genome_size);
            // DynamicModelGenerator generator = new DynamicModelGenerator();
            // population.visualize();
            visualize_individual(adn_eye, adn_head, adn_chest, adn_legs, adn_arms,population_size-j);
            string genomeString = string.Join("", individual.genome);
            // Debug.Log("Genome de l'individual : " + genomeString);
            // Debug.Log("Score de fitness de l'individual : " + individual.fitness_score);
        }

            debug_function(population);
    }

    // Update is called once per frame 
    void Update()
    {
        if (make_a_new_generation)
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            // Parcourir tous les GameObjects
            foreach (GameObject obj in allObjects)
            {
                // Vérifier si l'objet n'est pas la caméra et n'a pas d'autres tags/identifiants que vous voulez conserver
                if (!obj.CompareTag("MainCamera") && !obj.CompareTag("Model"))
                {
                    // Détruire l'objet
                    UnityEngine.Object.Destroy(obj);
                }
            }
            Properties wanted_properties = new Properties(strength,speed,health,vision,smart,resistance);
            MutationRate mutation_rate = new MutationRate(bit_mutation_rate, swap_mutation_rate, inversion_mutation_rate);
            foreach (Individual individual in population.individuals){
                individual.evaluate_fitness_score(wanted_properties);
            }
            population.new_generation(population_size, genome_size, fitness_algorithm, wanted_properties, mutation_rate);
            int i = 0;
            foreach (Individual individual in population.individuals)
            {
                string adn_eye = string.Join("", individual.genome.Take(4).Select(bit => bit.ToString()));
                string adn_head = string.Join("", individual.genome.Skip(4).Take(5).Select(bit => bit.ToString()));
                string adn_chest = string.Join("", individual.genome.Skip(8).Take(5).Select(bit => bit.ToString()));
                string adn_legs = string.Join("", individual.genome.Skip(12).Take(7).Select(bit => bit.ToString()));
                string adn_arms = string.Join("", individual.genome.Skip(18).Take(5).Select(bit => bit.ToString()));
                visualize_individual(adn_eye, adn_head, adn_chest, adn_legs, adn_arms, population_size - i);
                i++;

            }
            debug_function(population);
            make_a_new_generation = false;
        }
    }

    private void GenerateEyes(string adnEye, int position, Transform individual_transform)
    {
        //eyes
        int bitSize = Convert.ToInt32(adnEye[0].ToString() + adnEye[1].ToString(), 2);
        int bitNumber = Convert.ToInt32(adnEye[2].ToString() + adnEye[3].ToString(), 2);
        CreateEyes(bitSize, bitNumber, position, individual_transform);
    }
    private void GenerateHead(string adnHead, int position, Transform individual_transform)
    { //head
        int bitShape = Convert.ToInt32(adnHead[0].ToString() + adnHead[1].ToString(), 2);
        int bitDeformY = Convert.ToInt32(adnHead[2].ToString(), 2);
        int bitDeformZ = Convert.ToInt32(adnHead[3].ToString(), 2);
        CreateHead(bitShape, bitDeformY, bitDeformZ, position, individual_transform);
    }

    private void GenerateChest(string adnChest, int position, Transform individual_transform)
    {
        //chest
        int bitForm = Convert.ToInt32(adnChest[0].ToString() + adnChest[1].ToString(), 2);
        int bitSizeY = Convert.ToInt32(adnChest[2].ToString(), 2);
        int bitSizeZ = Convert.ToInt32(adnChest[3].ToString(), 2);
        CreateChest(bitForm, bitSizeY, bitSizeZ, position, individual_transform);
    }
    private void GenerateLegs(string adnLegs, int position, Transform individual_transform)
    {
        //legs
        int bitNumber = Convert.ToInt32(adnLegs[0].ToString() + adnLegs[1].ToString(), 2);
        int bitSize = Convert.ToInt32(adnLegs[2].ToString() + adnLegs[3].ToString() + adnLegs[4].ToString() + adnLegs[5].ToString(), 2);
        CreateLegs(bitSize, bitNumber, position, individual_transform);
    }
    private void GenerateArms(string adnArms, int position, Transform individual_transform)
    {
        //arms
        int bitNumber = Convert.ToInt32(adnArms[0].ToString() + adnArms[1].ToString(), 2);
        int bitSize = Convert.ToInt32(adnArms[2].ToString() + adnArms[3].ToString(), 2);
        CreateArms(bitSize, bitNumber, position, individual_transform);
    }

    private void CreateEyes(int size, int number, int position, Transform individual_transform)
    {
        float nSize = (size + 1) * 0.2f;
        GameObject eye = null;
        for (int i = 0; i <= number; i++)
        {
            if (number == 0)
            {
                eye = Instantiate(eyePrefab, new Vector3(0f+(float)position* 3.5f, 0f, 1f), Quaternion.identity);
            }
            if (number == 1)
            {
                eye = Instantiate(eyePrefab, new Vector3((i % 2) * (-0.6f) + ((i + 1) % 2) * 0.6f + (float)position * 3.5f, 0f, 1f), Quaternion.identity);
            }
            if (number == 2)
            {
                if (i < 2)
                {
                    eye = Instantiate(eyePrefab, new Vector3((i % 2) * (-0.6f) + ((i + 1) % 2) * 0.6f + (float)position * 3.5f, 0f, 1f), Quaternion.identity);
                }
                else
                {
                    eye = Instantiate(eyePrefab, new Vector3(0f + (float)position * 3.5f, 0.6f,1f), Quaternion.identity);
                }
            }
            if (number == 3)
            {
                eye = Instantiate(eyePrefab, new Vector3((i * 0.6f - (i % 2) * 0.6f) - 0.6f + (float)position * 3.5f, (i % 2f) * 0.6f + 0f, 1f), Quaternion.identity);
            }
            eye.transform.localScale = new Vector3(nSize, nSize, nSize);
            eye.transform.SetParent(individual_transform);
        }
    }

    private void CreateHead(int Shape, int DeformY, int DeformZ, int position, Transform individual_transform)
    {
        GameObject head;
        switch (Shape)
        {
            case 0:
                head = Instantiate(headPrefab1, new Vector3(0f + (float)position * 3.5f, 0f, 2f), Quaternion.identity);
                break;
            case 1:
                head = Instantiate(headPrefab2, new Vector3(0f + (float)position * 3.5f, 0f, 2f), Quaternion.identity);
                break;
            case 2:
                head = Instantiate(headPrefab3, new Vector3(0f + (float)position * 3.5f, 0f, 2f), Quaternion.identity);
                break;
            case 3:
                head = Instantiate(headPrefab4, new Vector3(0f + (float)position * 3.5f, 0f, 2f), Quaternion.identity);
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                head = Instantiate(headPrefab1, new Vector3(0f + (float)position * 3.5f, 0f, 1f), Quaternion.identity);
                break;
        }
        head.transform.localScale = new Vector3(2f + (float)DeformY, 2f + (float)DeformZ, 2f);
        head.transform.SetParent(individual_transform);
    }

    private void CreateChest(int bitForm, int bitSizeY, int bitSizeZ, int position, Transform individual_transform)
    {
        GameObject chest;
        switch (bitForm)
        {
            case 0:
                chest = Instantiate(chestPrefab1, new Vector3(0f + (float)position * 3.5f, -2f, 2f), Quaternion.identity);
                break;
            case 1:
                chest = Instantiate(chestPrefab2, new Vector3(0f + (float)position * 3.5f, -2f, 2f), Quaternion.identity);
                break;
            case 2:
                chest = Instantiate(chestPrefab3, new Vector3(0f + (float)position * 3.5f, -2f, 2f), Quaternion.identity);
                break;
            case 3:
                chest = Instantiate(chestPrefab4, new Vector3(0f + (float)position * 3.5f, -2f, 2f), Quaternion.identity);
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                chest = Instantiate(headPrefab1, new Vector3(0f + (float)position * 3.5f, 0f, 1f), Quaternion.identity);
                break;
        }
        chest.transform.localScale = new Vector3(2 + bitSizeY, 2 + bitSizeZ * 0.3f, 2);
        chest.transform.SetParent(individual_transform);
    }

    private void CreateLegs(int size, int number, int position, Transform individual_transform)
    {
        float nSize = 0.2f;
        GameObject leg;
        for (int i = 0; i <= number; i++)
        {
            leg = Instantiate(legPrefab, new Vector3(0.6f + (float)position * 3.5f, -(float)size * 0.2f-2.5f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            leg.transform.localScale = new Vector3(nSize, (float)size * 0.15f, nSize);
            leg.transform.SetParent(individual_transform);

            leg = Instantiate(legPrefab, new Vector3(-0.6f + (float)position * 3.5f, -(float)size * 0.2f - 2.5f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            leg.transform.localScale = new Vector3(nSize, (float)size * 0.15f, nSize);
            leg.transform.SetParent(individual_transform);
        }
    }

    private void CreateArms(int size, int number, int position, Transform individual_transform)
    {
        float nSize = 0.2f;
        GameObject arm;
        for (int i = 0; i <= number; i++)
        {
            arm = Instantiate(armPrefab, new Vector3(1.2f + (float)position * 3.5f, -1f - ((float)size + 1f) * 0.7f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            arm.transform.localScale = new Vector3(nSize, (float)size * 0.2f + 1f, nSize);
            arm.transform.localRotation = Quaternion.Euler(new Vector3(0f, 15f, 5f));
            arm.transform.SetParent(individual_transform);

            arm = Instantiate(armPrefab, new Vector3(-1.2f + (float)position * 3.5f, -1f - ((float)size + 1f) * 0.7f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            arm.transform.localScale = new Vector3(nSize, (float)size * 0.2f + 1f, nSize);
            arm.transform.localRotation = Quaternion.Euler(new Vector3(0f, -15f, -5f));
            arm.transform.SetParent(individual_transform);
        }
    }

    public void visualize_individual(string adnEye, string adnHead, string adnChest, string adnLegs, string adnArms, int position)
    {
        GameObject individualObject = new GameObject("Individual" + position);
        individualObject.transform.parent = modelObject.transform;
        individualObject.transform.localPosition = new Vector3(position * 3.5f, 0f, 0f);

        GenerateEyes(adnEye,position, individualObject.transform);
        GenerateHead(adnHead, position, individualObject.transform);
        GenerateChest(adnChest, position, individualObject.transform);
        GenerateLegs(adnLegs, position, individualObject.transform);
        GenerateArms(adnArms, position, individualObject.transform);
    }

    public class Properties{
        public bool strength;
        public bool speed;
        public bool health;
        public bool vision;
        public bool smart;
        public bool resistance;
        public Properties(bool _strength, bool _speed, bool _health, bool _vision, bool _smart, bool _resistance){
            strength=_strength;
            speed=_speed;
            health=_health;
            vision=_vision;
            smart=_smart;
            resistance=_resistance;
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

    // public enum Caracteristic
    // {
    //     strength = 19,
    //     speed = 14,
    //     health = 24,
    //     vision = 1,
    //     smart = 7,
    //     resistance = 10
    // }


    public enum FitnessAlgorithm
    {
        random,
        roulette_wheel,
    }

    public class Individual
    {
        public int[] genome;
        public int fitness_score;
        // public Dictionary<Caracteristic, int> caracteristics;
        private DynamicModelGenerator dynamicModelGenerator;

        public Individual(int[] initial_genome)
        {
            genome = initial_genome;

            // Initialiser les caractéristiques à partir du génome
            // caracteristics = new Dictionary<Caracteristic, int>
            // {
            //     { Caracteristic.strength, genome[(int)Caracteristic.strength] },
            //     { Caracteristic.speed, genome[(int)Caracteristic.speed] },
            //     { Caracteristic.health, genome[(int)Caracteristic.health] },
            //     { Caracteristic.vision, genome[(int)Caracteristic.vision] },
            //     { Caracteristic.smart, genome[(int)Caracteristic.smart] },
            //     { Caracteristic.resistance, genome[(int)Caracteristic.resistance] }
            // };

            // evaluate_fitness_score(properties);

            dynamicModelGenerator = new DynamicModelGenerator();
        }

        // void evaluate_caracteristics()
        // {
        //     caracteristics = new Dictionary<Caracteristic, int>
        //     {
        //         { Caracteristic.strength, genome[(int)Caracteristic.strength] },
        //         { Caracteristic.speed, genome[(int)Caracteristic.speed] },
        //         { Caracteristic.health, genome[(int)Caracteristic.health] },
        //         { Caracteristic.vision, genome[(int)Caracteristic.vision] },
        //         { Caracteristic.smart, genome[(int)Caracteristic.smart] },
        //         { Caracteristic.resistance, genome[(int)Caracteristic.resistance] }
        //     };
        // }

        // bool is_caracteristic(Caracteristic caracteristic)
        // {
        //     return caracteristics[caracteristic] == 1;
        // }

        // public void evaluate_fitness_score()
        // {
        //     fitness_score = 0;
        //     evaluate_caracteristics();
        //    foreach (Caracteristic caracteristic in Enum.GetValues(typeof(Caracteristic)))
        //    {
        //         if(is_caracteristic(caracteristic)){
        //             fitness_score+=1;
        //         }
        //    }
 
        // }


        public void evaluate_fitness_score(Properties properties){
            fitness_score=0;
            if(properties.speed){
                fitness_score+=genome[13]+genome[12];
                fitness_score+=(genome[14]+genome[15]+genome[16]+genome[17])*2;
            }
            if(properties.strength){
                fitness_score+=genome[18]+genome[19];
                fitness_score+=(genome[20]+genome[21])*2;
            }
            if(properties.health){
                fitness_score+=genome[22]+genome[23]+genome[24]+genome[25];
            }
            if(properties.resistance){
                fitness_score+=genome[9]+genome[8];
                fitness_score+=(genome[11]+genome[10])*2;
            }    
            if(properties.smart){
                fitness_score+=genome[4]+genome[5];
                fitness_score+=(genome[7]+genome[6])*2;
            }   
            if(properties.vision){
                fitness_score+=genome[0]+genome[1];
                fitness_score+=(genome[2]+genome[3])*2;
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
        public void swap_mutation(int genome_length, double mutation_rate)
        {
            for (int i = 0; i < genome_length; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    int index1 = UnityEngine.Random.Range(0, genome_length);
                    int index2 = UnityEngine.Random.Range(0, genome_length);

                    // Échanger les valeurs des bits à index1 et index2
                    int temp = genome[index1];
                    genome[index1] = genome[index2];
                    genome[index2] = temp;
                }
            }
        }


        //Mutation par inversion : Inverse l'ordre des gènes dans une partie du génome.
        public void inversion_mutation(int genome_length, double mutation_rate)
        {
            for (int i = 0; i < genome_length; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    int start = UnityEngine.Random.Range(0, genome_length);
                    int end = UnityEngine.Random.Range(start, genome_length);

                    // Inverse l'ordre des bits entre start et end inclus
                    while (start < end)
                    {
                        int temp = genome[start];
                        genome[start] = genome[end];
                        genome[end] = temp;
                        start++;
                        end--;
                    }
                }
            }
        }

        // Mutation de bit : Inverse le bit(0 devient 1, et vice versa) à un emplacement aléatoire du génome.
        public void bit_mutation(int genome_length, double mutation_rate)
        {
            for (int i = 0; i < genome_length; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutation_rate)
                {
                    // Inverser le bit (0 devient 1, 1 devient 0)
                    genome[i] = 1 - genome[i];
                }
            }
        }

        public void mutation(int genome_length, MutationRate mutation_rate)
        {
            bit_mutation(genome_length, mutation_rate.bit);
            swap_mutation(genome_length, mutation_rate.swap);
            inversion_mutation(genome_length, mutation_rate.inversion);
        }

        // public void visualize(){
        //     string adn_eye = string.Join("", genome.Take(4).Select(bit => bit.ToString()));
        //     Debug.Log("adn_eye : " + adn_eye);

        //     string adn_head = string.Join("", genome.Skip(4).Take(5).Select(bit => bit.ToString()));
        //     string adn_chest = string.Join("", genome.Skip(8).Take(5).Select(bit => bit.ToString()));
        //     string adn_legs = string.Join("", genome.Skip(12).Take(7).Select(bit => bit.ToString()));
        //     string adn_arms = string.Join("", genome.Skip(18).Take(5).Select(bit => bit.ToString()));
        //     dynamicModelGenerator.visualize_individual(adn_eye, adn_head, adn_chest, adn_legs, adn_legs);
        // }
    }

    public class Population
    {
        public List<Individual> individuals;

        public Population()
        {
            individuals = new List<Individual>();
        }

        private void Sort()
        {
            individuals.Sort((individu1, individu2) => individu2.fitness_score.CompareTo(individu1.fitness_score));
        }

        public void add_individual(Individual individual)
        {
            individuals.Add(individual);
            Sort();
        }

        public void pop_individual()
        {
            individuals.Remove(individuals.Last());
        }

        public void create_population(int population_length, int[] genome)
        {
            for (int i = 0; i < population_length; i++)
            {
                Individual individual = new Individual(genome);
                add_individual(individual);
            }
        }


        public void crossover(Individual parent1, Individual parent2, int population_length, int genome_length, Properties properties, MutationRate mutation_rate)
        {
            int crossover = UnityEngine.Random.Range(0, genome_length); // Point de croisement al atoire
            
            // Cr ez deux enfants en copiant les g nes des parents
            int[] genome1 = new int[26];
            int[] genome2 = new int[26];

            for (int i = 0; i < genome_length; i++)
            {
                genome1[i] = UnityEngine.Random.Range(0, 2); // Remplissez le tableau genome avec des 0 ou des 1
                genome2[i] = UnityEngine.Random.Range(0, 2); // Remplissez le tableau genome avec des 0 ou des 1
            }
            Individual child1 = new Individual(genome1);
            Individual child2 = new Individual(genome2);

            for (int i = 0; i < crossover; i++)
            {
                child1.genome[i] = parent1.genome[i];
                child2.genome[i] = parent2.genome[i];
            }

            for (int i = crossover; i < parent1.genome.Length; i++)
            {
                child1.genome[i] = parent2.genome[i];
                child2.genome[i] = parent1.genome[i];
            }

            // mutation
            child1.mutation(genome_length, mutation_rate);
            child2.mutation(genome_length, mutation_rate);

            // R  valuez les enfants (vous devez avoir une fonction de fitness appropri e)
            child1.evaluate_fitness_score(properties);
            child2.evaluate_fitness_score(properties);

            // evolution 

            if (child1.fitness_score >= child2.fitness_score)
            {
                evolve(child1, population_length);
                evolve(child2, population_length);
            }
            else
            {
                evolve(child2, population_length);
                evolve(child1, population_length);
            }

        }

        public void evolve(Individual child, int population_length)
        {
            add_individual(child); // Ajoutez le nouvel individu

            // Si la taille de la population dépasse la taille souhaitée, supprimez le moins adapté
            if (individuals.Count > population_length)
            {
                pop_individual();
            }
        }

        public void roulette_wheel_selection(out int index_parent1, out int index_parent2)
        {
            // Calculez la somme des scores de fitness de tous les individus
            int totalFitness = individuals.Sum(individual => individual.fitness_score);

            // Générez un nombre aléatoire entre 0 et la somme des scores de fitness
            int random_number_1 = UnityEngine.Random.Range(0, totalFitness);

            // Sélectionnez le premier parent
            index_parent1 = select_index_by_roulette(random_number_1);

            // Calculez la somme des scores de fitness des individus restants (en excluant le premier parent)
            int total_remaining_fitness = totalFitness - individuals[index_parent1].fitness_score;

            // Générez un nombre aléatoire pour sélectionner le deuxième parent parmi les individus restants
            int random_number_2 = UnityEngine.Random.Range(0, total_remaining_fitness);

            // Sélectionnez le deuxième parent
            index_parent2 = select_index_by_roulette(random_number_2, index_parent1);
        }

        // Fonction de sélection par roulette pour obtenir un indice
        private int select_index_by_roulette(int random_number, int excluded_index = -1)
        {
            int accumulated_fitness = 0;
            for (int i = 0; i < individuals.Count; i++)
            {
                if (i != excluded_index) // Excluez l'indice spécifié (si fourni)
                {
                    accumulated_fitness += individuals[i].fitness_score;
                    if (accumulated_fitness >= random_number)
                    {
                        return i; // Retournez l'indice sélectionné
                    }
                }
            }

            // En cas d'échec (ce qui ne devrait pas se produire normalement)
            return -1;
        }

        public void random_selection(out int index_parent1, out int index_parent2)
        {
            index_parent1 = UnityEngine.Random.Range(0, individuals.Count - 1);
            index_parent2 = UnityEngine.Random.Range(0, individuals.Count - 1);
            while (index_parent1 == index_parent2)
            {
                index_parent2 = UnityEngine.Random.Range(0, individuals.Count - 1);
            }
        }

        public void choose_parent(out Individual parent1, out Individual parent2, FitnessAlgorithm algorithm)
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
            parent1 = individuals[index_parent1];
            parent2 = individuals[index_parent2];
        }

        public void new_generation(int population_length, int genome_length, FitnessAlgorithm algorithm, Properties properties, MutationRate mutation_rate)
        {
            Individual parent1, parent2;
            choose_parent(out parent1, out parent2, algorithm);
            crossover(parent1, parent2, population_length, genome_length, properties, mutation_rate);
        }

        // public void visualize(){
        //     foreach(Individual individual in individuals){
        //         individual.visualize();
        //     }
        // }
    }

    public void debug_function(Population population)
    {
        // D bug
        Debug.Log("Nombre d'individus dans la population : " + population.individuals.Count);
        // Boucle pour parcourir chaque individual de la population et afficher son g nome.
        foreach (Individual individual in population.individuals)
        {
            // string genomeString = string.Join("", individual.genome); // Convertir le g nome en une cha ne de caract res
            // Debug.Log("Genome de l'individual : " + genomeString);
            Debug.Log("Score de fitness de l'individual : " + individual.fitness_score);
            // {
            // // Afficher les caractéristiques de l'individu
            // foreach (var caract in individual.caracteristics)
            // {
            //    Debug.Log($"Caractéristique : {caract.Key}, Valeur : {caract.Value}");
            // }
            // }
        }
    }

}

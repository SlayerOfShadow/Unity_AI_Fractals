using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor.Rendering;
using UnityEditor;

public class DynamicModelGenerator : MonoBehaviour
{


    void Start()
    {
      
    }

    // Update is called once per frame 
    void Update()
    {

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

    // class Capacity{
    //     public int value;
    //     public Capacity(v){
    //         value = value;
    //     }
    //     void update()
    // }



    // public class Individual
    // {
    //     public Genome genome;
    //     public int fitness_score;
    //     // Capacity speed;
    //     // public Dictionary<Caracteristic, int> caracteristics;
    //     // private DynamicModelGenerator dynamicModelGenerator;

    //     public Individual(Genome initial_genome)
    //     {
    //         genome = new Genome(initial_genome);

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
        //}

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



    }



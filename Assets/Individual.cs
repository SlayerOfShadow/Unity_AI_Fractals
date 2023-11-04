using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Individual : MonoBehaviour
{
    public int genome_size = 22;

    public class Genome{
        private int[] _value;
        public Genome(int[] value){
            _value = value;
        }
        public Genome(int size){
            _value = new int[size];
        }
        public Genome(Genome g){
            _value = g.Get();
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

    public Genome genome;
    public int fitness_score;

    // Start is called before the first frame update
    void Start()
    {
        genome = new Genome(genome_size);
        for (int i = 0; i < genome_size; i++)
        {
            genome.SetByIndex(i,UnityEngine.Random.Range(0, 2)); // Remplir le genome avec des 0 ou des 1
        }
        Debug.Log("Le Génome de l'individu est : "+ string.Join(", ", genome.Get()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

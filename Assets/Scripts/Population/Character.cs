using UnityEngine;
using System.Linq;

public class Character : MonoBehaviour
{
    public class Genome
    {
        private int[] _value;
        private int _size = 20;

        public Genome(int[] value)
        {
            _value = value;
        }

        public Genome()
        {
            _value = new int[_size];
        }

        public Genome(Genome g)
        {
            _value = g.Get();
        }

        public void Set(Genome g)
        {
            for (int i = 0; i < _size; i++)
            {
                _value[i] = g.GetIndex(i);
            }
        }

        public int Size()
        {
            return _size;
        }

        public int[] Get()
        {
            return _value;
        }

        public int GetIndex(int i)
        {
            return _value[i];
        }

        public int GetIndex(Population.GenomeInformations i)
        {
            return _value[(int)i];
        }

        public void SetByIndex(int i, int value)
        {
            _value[i] = value;
        }

        public int[] PartialGenome(int start, int number_of_elements)
        {
            return Enumerable.Range(start, number_of_elements).Select(i => _value[i]).ToArray();
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
        public void SwapMutation(double mutationRate)
        {
            for (int i = 0; i < _size; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                {
                    int index1 = UnityEngine.Random.Range(0, _size);
                    int index2 = UnityEngine.Random.Range(0, _size);

                    // Échanger les valeurs des bits à index1 et index2
                    int temp = _value[index1];
                    _value[index1] = _value[index2];
                    _value[index2] = temp;
                }
            }
        }

        // Mutation par inversion : Inverse l'ordre des gènes dans une partie du génome.
        public void InversionMutation(double mutationRate)
        {
            for (int i = 0; i < _size; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                {
                    int start = UnityEngine.Random.Range(0, _size);
                    int end = UnityEngine.Random.Range(start, _size);

                    // Inverse l'ordre des bits entre start et end inclus
                    while (start < end)
                    {
                        int temp = _value[start];
                        _value[start] = _value[end];
                        _value[end] = temp;
                        start++;
                        end--;
                    }
                }
            }
        }

        // Mutation de bit : Inverse le bit(0 devient 1, et vice versa) à un emplacement aléatoire du génome.
        public void BitMutation(double mutationRate)
        {
            for (int i = 0; i < _size; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                {
                    // Inverser le bit (0 devient 1, 1 devient 0)
                    _value[i] = 1 - _value[i];
                }
            }
        }
    }

    public class Capacities
    {
        public bool vision;
        public bool smart;
        public bool resistance;
        public bool strength;
        public bool speed;

        public Capacities(bool _vision, bool _smart, bool _resistance, bool _strength, bool _speed)
        {
            vision = _vision;
            smart = _smart;
            resistance = _resistance;
            strength = _strength;
            speed = _speed;
        }
    }

    public class CapacitiesStatistics
    {
        public int strength;
        public int speed;
        public int health;
        public int vision;
        public int smart;
        public int resistance;

        public CapacitiesStatistics()
        {
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

        public MutationRate(double bitMutation, double swapMutation, double inversionMutation)
        {
            bit = bitMutation;
            swap = swapMutation;
            inversion = inversionMutation;
        }
    }

    public class Individual
    {
        private Genome _genome;
        private int _individualId;
        private int _fitnessScore;
        private int _remainingLife = 9000; // frame
        private CapacitiesStatistics _statistics = new CapacitiesStatistics(); // en fonction des gènes de l'individu il aura des stats de capacités différentes

        public Individual()
        {
            GenerateGenome();
            EvaluateStatistics();
        }

        public Individual(Genome g)
        {
            _genome = new Genome();
            _genome.Set(g);
        }

        public Genome GetGenome()
        {
            return _genome;
        }

        public int GetGenomeByIndex(int i)
        {
            return _genome.GetIndex(i);
        }

        public void SetGenomeByIndex(int i, int value)
        {
            _genome.SetByIndex(i, value);
        }

        public int GenomeSize()
        {
            return _genome.Size();
        }

        public int GetId()
        {
            return _individualId;
        }

        public void SetId(int id)
        {
            _individualId = id;
        }

        public int GetFitnessScore()
        {
            return _fitnessScore;
        }

        public int GetRemainingLife()
        {
            return _remainingLife;
        }

        public void UpdateRemainingLife()
        {
            _remainingLife--;
        }

        public bool isDead()
        {
            return (_remainingLife <= 0);
        }

        private void EvaluateStatistics()
        {
            _statistics.vision = 
                _genome.GetIndex(Population.GenomeInformations.eyeSize1)
                + _genome.GetIndex(Population.GenomeInformations.eyeSize2)
                + (_genome.GetIndex(Population.GenomeInformations.eyeNumber1)
                    + _genome.GetIndex(Population.GenomeInformations.eyeNumber2)) * 2;
            _statistics.smart = 
                _genome.GetIndex(Population.GenomeInformations.headShape1)
                + _genome.GetIndex(Population.GenomeInformations.headShape2)
                + _genome.GetIndex(Population.GenomeInformations.headDeformByY)
                + _genome.GetIndex(Population.GenomeInformations.headDeformByZ);
            _statistics.resistance = 
                _genome.GetIndex(Population.GenomeInformations.chestShape1)
                + _genome.GetIndex(Population.GenomeInformations.chestShape2)
                + _genome.GetIndex(Population.GenomeInformations.chestDeformByY)
                + _genome.GetIndex(Population.GenomeInformations.chestDeformByZ);
            _statistics.strength = 
                (_genome.GetIndex(Population.GenomeInformations.armSize1)
                    + _genome.GetIndex(Population.GenomeInformations.armSize2)) * 2
                + _genome.GetIndex(Population.GenomeInformations.armNumber1)
                + _genome.GetIndex(Population.GenomeInformations.armNumber2);
            _statistics.speed = 
                (_genome.GetIndex(Population.GenomeInformations.legSize1)
                    + _genome.GetIndex(Population.GenomeInformations.legSize2)) * 2
                + _genome.GetIndex(Population.GenomeInformations.legNumber1)
                + _genome.GetIndex(Population.GenomeInformations.legNumber2);
        }

        public void EvaluateFitnessScore(Capacities capacitiesWanted){
            _fitnessScore = 0;
            if(capacitiesWanted.vision)
            {
                _fitnessScore += _statistics.vision;
            }     
            if(capacitiesWanted.smart){
                _fitnessScore += _statistics.smart;
            }   
            if(capacitiesWanted.resistance){
                _fitnessScore += _statistics.resistance;
            }    
            if(capacitiesWanted.strength){
                _fitnessScore += _statistics.strength;
            }
            if(capacitiesWanted.speed)
            {
                _fitnessScore += _statistics.speed;
            }
        }

        public void GenerateGenome()
        {
            _genome = new Genome();
            for (int i = 0; i < _genome.Size(); i++)
            {
                _genome.SetByIndex(i, UnityEngine.Random.Range(0, 2)); // Remplir le genome avec des 0 ou des 1
            }
            Debug.Log("Le Génome de l'individu est : "+ string.Join(", ", _genome.Get()));
        }

        public void Mutation(MutationRate mutationRate)
        {
            _genome.BitMutation(mutationRate.bit);
            _genome.SwapMutation(mutationRate.swap);
            _genome.InversionMutation(mutationRate.inversion);
            EvaluateStatistics();
        }

        public void DebugIndividual(){
            Debug.Log("Génome :" + _genome);
            Debug.Log("Score de fitness : " + _fitnessScore);
            Debug.Log("Espérence de vie : " + _remainingLife);
            Debug.Log("Statistiques : ");
            Debug.Log("Vision : " + _statistics.vision);
            Debug.Log("Intelligence : " + _statistics.smart);
            Debug.Log("Résistance : " + _statistics.resistance);
            Debug.Log("Force : " + _statistics.strength);
            Debug.Log("Vitesse : " + _statistics.speed);
        }
    }
}

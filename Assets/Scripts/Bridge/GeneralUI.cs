using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUI : MonoBehaviour
{
    [SerializeField] GameOfLife[] gameOfLives;
    [SerializeField] Slider generalSlider;
    [SerializeField] Text populationCount;
    [SerializeField] Population population;
    int totalMaxIterations = 0;
    public int totalCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameOfLife gof in gameOfLives)
        {
            totalMaxIterations += gof.maxIterations;
        }
    }

    public void UpdateSlider()
    {
        generalSlider.value = (float)totalCount / totalMaxIterations;
        
    }

    void Update()
    {
        populationCount.text = population.PopulationSize().ToString();
    }
}

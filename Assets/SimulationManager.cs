using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] Population population;

    void Update()
    {
        if (population.PopulationSize() == 0)
        {
            EndOfSimulation();
        }
    }

    public float delayBeforeLoad = 3f;

    public void EndOfSimulation()
    {
        StartCoroutine(LoadSceneWithDelay("EndScene"));
    }

    IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneName);
    }
}
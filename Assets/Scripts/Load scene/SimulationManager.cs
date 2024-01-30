using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] Population population;
    [SerializeField] GameOfLife gameOfLife;
    public float delayBeforeLoadDeadScene = 5f;
    public float delayBeforeLoadWinScene = 15f;

    void Update()
    {
        if (population.PopulationSize() == 0)
        {
            StartCoroutine(LoadSceneWithDelay("DeadScene", delayBeforeLoadDeadScene));
        }
        if (!gameOfLife.canBuild)
        {
            StartCoroutine(LoadSceneWithDelay("WinScene", delayBeforeLoadWinScene));
        }
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
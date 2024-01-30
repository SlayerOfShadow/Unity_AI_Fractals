using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] Population population;
    [SerializeField] GameOfLife gameOfLife;
    public float delayBeforeLoadDeadScene = 3f;
    public float delayBeforeLoadWinScene = 3f;

    void Update()
    {
        if (population.PopulationSize() == 0)
        {
            StartCoroutine(LoadSceneWithDelay("DeadScene"));
        }
        if (!gameOfLife.trueBridge)
        {
            StartCoroutine(LoadSceneWithDelay("WinScene"));
        }
    }

    IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoadDeadScene);
        SceneManager.LoadScene(sceneName);
    }
}
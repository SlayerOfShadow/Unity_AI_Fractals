using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartSimulation()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitSimulation()
    {
        Application.Quit();
    }
}

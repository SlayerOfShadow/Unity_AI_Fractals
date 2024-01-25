using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject informationsPanel;

    public void StartSimulation()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitSimulation()
    {
        Application.Quit();
    }

    public void SetInformationsPanel(bool b)
    {
        informationsPanel.SetActive(b);
    }
}

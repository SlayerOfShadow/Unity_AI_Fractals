using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndManager : MonoBehaviour
{
    public float timeBeforeMainMenu = 10f;

    void Start()
    {
        StartCoroutine(ReturnToMainMenuAfterDelay());
    }

    IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(timeBeforeMainMenu);
        SceneManager.LoadScene("StartScene");
    }
}

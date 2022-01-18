using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
    public void Exit() => Application.Quit();
}

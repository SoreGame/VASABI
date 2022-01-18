using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    Animator anim;

    public string newGameSceneName;

    [Header("Options Panel")]
    public GameObject StartGameOptionsPanel;

    private void Start () 
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        anim = GetComponent<Animator>();
    }

    public void openStartGameOptions()
    {
        StartGameOptionsPanel.SetActive(true);
        anim.Play("buttonTweenAnims_on");
    }

    public void newGame()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
            SceneManager.LoadScene(newGameSceneName);
        else
            Debug.Log("Add scene name");
    }

    public void back_options()
    {
        anim.Play("buttonTweenAnims_off");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
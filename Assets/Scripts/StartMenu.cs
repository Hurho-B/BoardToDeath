using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    private void Awake()
    {
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    //currently just switches scenes
    //later on should disable the main menu
    //and let the skater freeroam in hub area
    public void StartGame()
    {
        SceneManager.LoadScene("BtD PROTOTYPE SCENE");
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
    }

    public void OpenSettings()
    {
        startMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterfaceManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    bool start;
    bool pause;
    bool settings;

    private void Awake()
    {
        startMenu.SetActive(true);
        start = true;

        pauseMenu.SetActive(false);
        pause = false;

        settingsMenu.SetActive(false);
        settings = false;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && !start && !settings)
        {
            pauseMenu.SetActive(true);
        }
    }

    //currently just switches scenes
    //later on should disable the main menu
    //and let the skater freeroam in hub area
    public void StartGame()
    {
        startMenu.SetActive(false);
        start = false;

        SceneManager.LoadScene("BtD PROTOTYPE SCENE");
    }
    public void OpenSettings()
    {
        startMenu.SetActive(false);
        start = false;
        settingsMenu.SetActive(true);
        settings = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToStart()
    {
        startMenu.SetActive(true);
        start = true;
        settingsMenu.SetActive(false);
        settings = false;
    }

}

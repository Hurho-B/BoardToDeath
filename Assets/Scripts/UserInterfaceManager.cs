using Unity.VisualScripting;
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
        if (Input.GetKeyDown(KeyCode.Escape) && !start && !settings)
        {
            if (!pause)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                pause = true;
            }
            else if (pause)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
                pause = false;
            }
        }
    }

    //currently just switches scenes
    //later on should disable the main menu
    //and let the skater freeroam in hub area
    public void StartGame()
    {
        startMenu.SetActive(false);
        start = false;

        SceneManager.LoadScene("ShowcaseLevel");
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
        SceneManager.LoadScene("StartZone");
        Time.timeScale = 1;
        Destroy(gameObject);
    }

}

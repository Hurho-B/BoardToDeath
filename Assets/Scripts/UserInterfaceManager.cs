using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterfaceManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    public bool start;
    public bool pause;
    public bool settings;

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

    public void Restart()
    {
        pauseMenu.SetActive(false);
        pause = false;

        SceneManager.LoadScene("ShowcaseLevel");
        Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        settings = true;

        if (SceneManager.GetActiveScene().name == "StartZone")
        {
            startMenu.SetActive(false);
            start = false;
        }
        else
        {
            pauseMenu.SetActive(false);
            pause = false;
        }
    }

    public void QuitGame()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Back()
    {
        settingsMenu.SetActive(false);
        settings = false;

        if (SceneManager.GetActiveScene().name == "StartZone")
        {
            startMenu.SetActive(true);
            start = true;
        }
        else
        {
            pauseMenu.SetActive(true);
            pause = true;
        }
    }

    public void BackToStart()
    {
        SceneManager.LoadScene("StartZone");
        Time.timeScale = 1;
        Destroy(gameObject);
    }
}

using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    public MenuSoundManager menuSoundManager;
    public MusicHandler MusicHandler;

    public float maxMasterVolume;
    public float currentMasterVolume;
    public float maxMusicVolume;
    public float currentMusicVolume;
    public float maxSoundVolume;
    public float currentSoundVolume;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    public bool start;
    public bool pause;
    public bool settings;

    private void Awake()
    {
        maxMasterVolume = 1f;
        currentMasterVolume = 1f;
        maxMusicVolume = .75f;
        currentMusicVolume = .75f;
        maxSoundVolume = 1f;
        currentSoundVolume = 1f;

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
                //Applies a filter and gain reduction to the BGM when the player pauses
                MusicHandler.loPass.cutoffFrequency = 2000;
                MusicHandler.hiPass.cutoffFrequency = 1000;

                //Prevents sounds from overlapping by stopping the currently playing sound before playing another one
                if (!menuSoundManager.MenuSoundPlayer.isPlaying)
                    menuSoundManager.PlayPauseSound();
                else
                {
                    menuSoundManager.MenuSoundPlayer.Stop();
                    menuSoundManager.PlayPauseSound();
                }

                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                pause = true;

                changeMusicVol();
            }
            else if (pause)
            {
                //Restores normal sound quality to the BGM when the player unpauses
                MusicHandler.loPass.cutoffFrequency = 22000;
                MusicHandler.hiPass.cutoffFrequency = 10;

                //Prevents sounds from overlapping by stopping the currently playing sound before playing another one
                if (!menuSoundManager.MenuSoundPlayer.isPlaying)
                    menuSoundManager.PlayUnpauseSound();
                else
                {
                    menuSoundManager.MenuSoundPlayer.Stop();
                    menuSoundManager.PlayUnpauseSound();
                }

                pauseMenu.SetActive(false);
                Time.timeScale = 1;
                pause = false;

                changeMusicVol();
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
        MusicHandler.loPass.cutoffFrequency = 22000;
        MusicHandler.hiPass.cutoffFrequency = 10;

        pauseMenu.SetActive(false);
        pause = false;

        changeMusicVol();

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

    public void changeMasterVol()
    {
        float multiplier = masterSlider.value;

        currentMasterVolume = maxMasterVolume * multiplier;

        changeMusicVol();
        changeSoundVol();
    }

    public void changeMusicVol()
    {
        float multiplier = musicSlider.value * currentMasterVolume;
        float pausedMultiplier = 1f;

        if ((pause || settings) && (SceneManager.GetActiveScene().name != "StartZone"))
        {
            pausedMultiplier = .67f;
        }

        currentMusicVolume = maxMusicVolume * multiplier * pausedMultiplier;
        MusicHandler.MusicPlayer.volume = currentMusicVolume;
    }

    public void changeSoundVol()
    {
        float multiplier = soundSlider.value * currentMasterVolume;

        currentSoundVolume = maxSoundVolume * multiplier;
        menuSoundManager.MenuSoundPlayer.volume = currentSoundVolume;
    }
}

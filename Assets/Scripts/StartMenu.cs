using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    //currently just switches scenes
    //later on should disable the main menu
    //and let the skater freeroam in hub area
    public void StartGame()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("BtD PROTOTYPE SCENE");
    }

    public void OpenSettings()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }


}

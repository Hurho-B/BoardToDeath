using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FailState : MonoBehaviour
{
    public Image FailPanel;
    public float newAlpha;

    Scene activeScene;

    float timeElapsed = 0;
    float lerpDuration = 2;

    float startValue = 0;
    float endValue = 1;
    float valueToLerp;

    public bool isDead = false;
    public bool shouldRestart = false;

    public float lerpRatio = 0f;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (isDead == true)
        {
            if (timeElapsed < lerpDuration)
            {
                lerpRatio += Time.deltaTime / lerpDuration;

                newAlpha = Mathf.Lerp(startValue, endValue, lerpRatio);
                timeElapsed += Time.deltaTime;

                Color newColor = FailPanel.color;
                newColor.a = newAlpha;
                FailPanel.color = newColor;
            }
        }

        if (newAlpha == 1)
        {
            shouldRestart = true;
        }

        if (shouldRestart == true)
        {
            shouldRestart = false;
            isDead = false;
            SceneManager.LoadScene(activeScene.name);
        }
    }
}
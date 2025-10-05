using UnityEngine;
using UnityEngine.UI;

public class SliderScripts : MonoBehaviour
{
    public Slider thisSlider;
    public float sliderSpeed;
    public CanvasGroup uiGroup;
    public bool performingManual = false;
    public FailState failState;

    public void Start()
    {
        uiGroup.alpha = 0f;

        Debug.Log(thisSlider.value);
        failState = GetComponent<FailState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            performingManual = true;
            sliderSpeed = 5f;
            uiGroup.alpha = 1f;
        }

        if (performingManual)
        {
            thisSlider.value += sliderSpeed * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.DownArrow) && sliderSpeed > 0 || Input.GetKeyDown(KeyCode.UpArrow) && sliderSpeed < 0)
            {
                sliderSpeed *= -1.25f;
            }

            if (thisSlider.value <= -10 || thisSlider.value >= 10)
            {
                uiGroup.alpha = 0f;
                thisSlider.value = 0;
                failState.isDead = true;
                performingManual = false;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                uiGroup.alpha = 0f;
                thisSlider.value = 0;
                performingManual = false;
            }
        }
    }
}

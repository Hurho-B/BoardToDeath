using UnityEngine;
using UnityEngine.UI;

public class SliderScripts : MonoBehaviour
{
    public Slider thisSlider;
    private float sliderSpeed = 0f;
    public CanvasGroup uiGroup;
    private bool performingManual = false;
    private FailState failState;

    private void Start()
    {
        uiGroup.alpha = 0f;
        failState = GetComponent<FailState>();
    }

    // Update is called once per frame
    private void Update()
    {

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
        }
    }

    public void ToggleManual(bool boolValue)
    {
        if (boolValue)
        {
            performingManual = true;
            uiGroup.alpha = 1f;
            sliderSpeed = 5f;
        }
        else
        {
            performingManual = false;
            uiGroup.alpha = 0f;
            thisSlider.value = 0;
        }
    }

}

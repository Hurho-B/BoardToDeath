using UnityEngine;
using UnityEngine.UI;

public class SliderScripts : MonoBehaviour
{
    public Slider thisSlider;
    public float sliderSpeed;
    public bool performingManual = false;

    public void Start()
    {
        Debug.Log(thisSlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            performingManual = true;
            sliderSpeed = 5f;
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
                thisSlider.value = 0;
                performingManual = false;
            }
        }
    }
}

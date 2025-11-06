using UnityEngine;
using TMPro;

public class UITextHeartbeat : MonoBehaviour
{
    [Header("References")]
    public TMP_Text text; 

    [Header("Heartbeat Settings")]
    public float minScale = 1f;          
    public float maxScale = 1.15f;       
    public float beatSpeed = 1.2f;       
    public float restDuration = 0.3f;    

    [Header("Optional Color Pulse")]
    public bool colorPulse = true;       
    public Color pulseColor = new Color(1f, 0.3f, 0.3f); 
    private Color originalColor;

    private float timer;
    private bool isBeatingUp = true;
    private bool resting = false;

    private void Awake()
    {
        if (!text) text = GetComponent<TMP_Text>();
        originalColor = text.color;
    }

    private void Update()
    {
        if (resting)
        {
            timer += Time.deltaTime;
            if (timer >= restDuration)
            {
                resting = false;
                timer = 0f;
            }
            return;
        }

        // Heartbeat pulse
        float t = Mathf.PingPong(Time.time * beatSpeed, 1f);
        float scale = Mathf.Lerp(minScale, maxScale, EaseOutElastic(t));
        transform.localScale = new Vector3(scale, scale, scale);

        // Optional color pulse
        if (colorPulse)
        {
            text.color = Color.Lerp(originalColor, pulseColor, t);
        }

        // Detect peak and rest between beats
        if (t > 0.98f && !resting)
        {
            resting = true;
            timer = 0f;
        }
    }

    // Elastic ease for a nice organic "beat" motion
    private float EaseOutElastic(float x)
    {
        float c4 = (2f * Mathf.PI) / 3f;
        return x == 0
            ? 0
            : x == 1
            ? 1
            : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
    }
}


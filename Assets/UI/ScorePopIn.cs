using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopIn : MonoBehaviour
{
    [Header("References")]
    public TMP_Text scoreText;

    [Header("Animation Settings")]
    public float startScale = 5f;
    public float endScale = 132f;
    public float fadeDuration = 0.5f;
    public float shrinkDuration = 0.25f;

    [Header("Rattle Settings")]
    public float rattleDuration = 0.4f;
    public float rattleIntensity = 10f;
    public int rattleFrequency = 40;

    private Vector3 targetScale;
    private Color originalColor;
    private Vector3 originalPos;

    void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponent<TMP_Text>();

        originalColor = scoreText.color;
        originalPos = transform.localPosition;

        // Hide text initially
        Color transparent = originalColor;
        transparent.a = 0;
        scoreText.color = transparent;

        // Set starting scale
        transform.localScale = Vector3.one * startScale;
    }

    void OnEnable()
    {
        StartCoroutine(PopInSequence());
    }

    private IEnumerator PopInSequence()
    {
        // Step 1: Fade in + shrink to end scale
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade in
            Color newColor = scoreText.color;
            newColor.a = Mathf.Lerp(0, originalColor.a, t);
            scoreText.color = newColor;

            // Shrink from startScale → endScale
            float scale = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // Ensure final values
        transform.localScale = Vector3.one * endScale;
        scoreText.color = originalColor;

        // Step 2: Rattle after impact
        yield return StartCoroutine(RattleEffect());
    }

    private IEnumerator RattleEffect()
    {
        float elapsed = 0f;

        while (elapsed < rattleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rattleDuration;
            float intensity = rattleIntensity * (1f - t);

            
            float offsetX = Mathf.Sin(Time.time * rattleFrequency) * intensity * Random.Range(0.5f, 1f);
            float offsetY = Mathf.Cos(Time.time * rattleFrequency * 1.3f) * intensity * Random.Range(0.5f, 1f);

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    
    public void Play()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.one * startScale;
        Color transparent = originalColor;
        transparent.a = 0;
        scoreText.color = transparent;
        transform.localPosition = originalPos;
        StartCoroutine(PopInSequence());
    }
}





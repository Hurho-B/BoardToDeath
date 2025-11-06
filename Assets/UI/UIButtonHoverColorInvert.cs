using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class UIButtonHoverColorInvert : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public TMP_Text buttonText;

    [Header("Scale Settings")]
    public float normalScale = 1f;
    public float hoverScale = 1.1f;
    public float scaleSpeed = 6f;
    public float breathingAmplitude = 0.05f;
    public float breathingSpeed = 2f;

    [Header("Color Settings")]
    public Color normalFaceColor = Color.white;
    public Color normalOutlineColor = Color.black;
    public float colorPulseStrength = 0.08f; 

    private bool isHovering = false;
    private Coroutine breathingRoutine;
    private float breathingTimer = 0f;

    private Material buttonMaterial;
    private Color invertedFaceColor;
    private Color invertedOutlineColor;
    private Color currentFaceColor;
    private Color currentOutlineColor;

    private void Reset()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        // Create unique material instance
        buttonMaterial = buttonText.fontMaterial;

        // Store inverted colors
        invertedFaceColor = normalOutlineColor;
        invertedOutlineColor = normalFaceColor;

        // Initial appearance
        SetTMPColors(normalFaceColor, normalOutlineColor);
        transform.localScale = Vector3.one * normalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // Invert colors
        currentFaceColor = invertedFaceColor;
        currentOutlineColor = invertedOutlineColor;
        SetTMPColors(currentFaceColor, currentOutlineColor);

        // Start breathing
        if (breathingRoutine != null)
            StopCoroutine(breathingRoutine);
        breathingRoutine = StartCoroutine(BreathingEffect());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // Restore colors
        currentFaceColor = normalFaceColor;
        currentOutlineColor = normalOutlineColor;
        SetTMPColors(currentFaceColor, currentOutlineColor);

        // Stop breathing, return to normal
        if (breathingRoutine != null)
            StopCoroutine(breathingRoutine);
        StartCoroutine(ReturnToNormalScale());
    }

    private void SetTMPColors(Color face, Color outline)
    {
        if (buttonMaterial == null)
            buttonMaterial = buttonText.fontMaterial;

        buttonMaterial.SetColor(ShaderUtilities.ID_FaceColor, face);
        buttonMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outline);
        buttonText.UpdateMeshPadding();
        buttonText.SetMaterialDirty();
    }

    private IEnumerator BreathingEffect()
    {
        breathingTimer = 0f;

        while (isHovering)
        {
            breathingTimer += Time.deltaTime * breathingSpeed;

            // Breathing scale
            float scaleOffset = Mathf.Sin(breathingTimer) * breathingAmplitude;
            float currentScale = hoverScale + scaleOffset;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * currentScale, Time.deltaTime * scaleSpeed);

            // Neon pulse for both face and outline
            float pulse = (Mathf.Sin(breathingTimer) + 1f) * 0.5f; // 0–1
            float brightness = Mathf.Lerp(1f - colorPulseStrength, 1f + colorPulseStrength, pulse);

            Color pulsedFace = currentFaceColor * brightness;
            Color pulsedOutline = currentOutlineColor * brightness;

            SetTMPColors(pulsedFace, pulsedOutline);

            yield return null;
        }
    }

    private IEnumerator ReturnToNormalScale()
    {
        while (!isHovering && Vector3.Distance(transform.localScale, Vector3.one * normalScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * normalScale, Time.deltaTime * scaleSpeed);
            yield return null;
        }

        transform.localScale = Vector3.one * normalScale;
    }
}








using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Button))]
public class UIButtonHoverColorInvert : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public TMP_Text buttonText; // assign your TMP text in inspector

    [Header("Scale Settings")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 8f;

    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    private Color originalFaceColor;
    private Color originalOutlineColor;

    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        // IMPORTANT: create a unique material instance for this text so we can modify it independently
        buttonText.fontMaterial = new Material(buttonText.fontMaterial);

        // Cache original colors
        originalFaceColor = buttonText.color;
        originalOutlineColor = buttonText.fontMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StartScaleTo(originalScale * hoverScale);
        InvertTextColors();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        StartScaleTo(originalScale);
        RestoreOriginalColors();
    }

    private void StartScaleTo(Vector3 target)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine(target));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * scaleSpeed);
            yield return null;
        }
        transform.localScale = target;
    }

    private void InvertTextColors()
    {
        // Invert face color (white <-> black)
        Color invertedFace = InvertColor(originalFaceColor);
        buttonText.color = invertedFace;

        // Invert outline color (black <-> white)
        Color invertedOutline = InvertColor(originalOutlineColor);
        buttonText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, invertedOutline);
    }

    private void RestoreOriginalColors()
    {
        buttonText.color = originalFaceColor;
        buttonText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, originalOutlineColor);
    }

    private Color InvertColor(Color color)
    {
        return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
    }
}



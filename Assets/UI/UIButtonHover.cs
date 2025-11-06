using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public ButtonTextShake textShake;  

    [Header("Scale Settings")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 8f;

    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    void Start()
    {
        originalScale = transform.localScale;
        if (textShake == null)
            textShake = GetComponentInChildren<ButtonTextShake>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleTo(originalScale * hoverScale);
        if (textShake != null)
            textShake.TriggerShake();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleTo(originalScale);
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
}

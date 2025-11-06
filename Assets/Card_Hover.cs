using UnityEngine;
using UnityEngine.EventSystems;

public class UICardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float scaleAmount = 1.08f;         // How much the card scales on hover
    public float tiltAmount = 10f;            // Max tilt angle in degrees
    public float speed = 8f;                  // How fast it scales/rotates
    public bool bounce = true;                // Optional quick pop effect

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Quaternion originalRotation;
    private Quaternion targetRotation;

    private RectTransform rectTransform;
    private bool isHovering;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;

        targetScale = originalScale;
        targetRotation = originalRotation;
    }

    void Update()
    {
        // Smooth scale and rotation transitions
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * speed);
        rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, targetRotation, Time.deltaTime * speed);

        // Continuously update tilt while hovering
        if (isHovering)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localMousePos);

            // Normalize the mouse position relative to card size
            Vector2 normalized = new Vector2(
                Mathf.Clamp(localMousePos.x / (rectTransform.rect.width * 0.5f), -1f, 1f),
                Mathf.Clamp(localMousePos.y / (rectTransform.rect.height * 0.5f), -1f, 1f)
            );

            // Compute target rotation (tilt toward cursor)
            targetRotation = originalRotation * Quaternion.Euler(-normalized.y * tiltAmount, normalized.x * tiltAmount, 0f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = originalScale * scaleAmount;

        if (bounce)
            rectTransform.localScale = originalScale * (scaleAmount + 0.05f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = originalScale;
        targetRotation = originalRotation;
    }
}



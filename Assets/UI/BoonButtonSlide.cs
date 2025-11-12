using System.Collections;
using UnityEngine;

public class UICardDealer : MonoBehaviour
{
    [Header("Card Deal Settings")]
    public RectTransform[] cards;       // assign your 3 card buttons here
    public float dealDelay = 0.3f;      // time between each card being dealt
    public float dealDuration = 0.8f;   // how long each deal animation takes
    public float startOffsetY = 100f;   // distance above bottom of screen
    public float rotationAngle = 10f;   // random rotation per deal
    public float overshoot = 1.1f;      // bounce amount

    private Vector2[] finalPositions;
    private Vector2 dealStartPos;

    void Start()
    {
        if (cards == null || cards.Length == 0)
        {
            Debug.LogWarning("UICardDealer: No cards assigned!");
            return;
        }

        // Find Canvas space dimensions
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        dealStartPos = new Vector2(0, -canvasRect.rect.height / 2f + startOffsetY);

        // Store each card's final resting position first
        finalPositions = new Vector2[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            finalPositions[i] = cards[i].anchoredPosition;
        }

        // Immediately move all cards to bottom center and hide
        foreach (RectTransform card in cards)
        {
            CanvasGroup group = card.GetComponent<CanvasGroup>();
            if (group == null)
                group = card.gameObject.AddComponent<CanvasGroup>();

            card.anchoredPosition = dealStartPos;
            group.alpha = 0f;
        }

        // Start dealing animation
        StartCoroutine(DealCards());
    }

    private IEnumerator DealCards()
    {
        yield return null; // wait one frame to ensure layout is ready

        for (int i = 0; i < cards.Length; i++)
        {
            StartCoroutine(AnimateCard(cards[i], finalPositions[i]));
            yield return new WaitForSeconds(dealDelay);
        }
    }

    private IEnumerator AnimateCard(RectTransform card, Vector2 endPos)
    {
        CanvasGroup group = card.GetComponent<CanvasGroup>();
        Vector2 startPos = dealStartPos;
        float timer = 0f;
        float randomRot = Random.Range(-rotationAngle, rotationAngle);

        // Reset before anim
        card.anchoredPosition = startPos;
        card.localRotation = Quaternion.Euler(0, 0, randomRot);
        group.alpha = 0f;

        while (timer < dealDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / dealDuration);
            float eased = EaseOutBack(t, overshoot);

            card.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            card.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(randomRot, 0f, eased));
            group.alpha = Mathf.Lerp(0f, 1f, eased);

            yield return null;
        }

        // Snap to final position cleanly
        card.anchoredPosition = endPos;
        card.localRotation = Quaternion.identity;
        group.alpha = 1f;
    }

    private float EaseOutBack(float x, float overshoot)
    {
        float c1 = overshoot * 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}







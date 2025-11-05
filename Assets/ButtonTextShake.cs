using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ButtonTextShake : MonoBehaviour
{
    private TMP_Text tmpText;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private Mesh mesh;

    [Header("Shake Settings")]
    public float magnitude = 3f;
    public float speed = 25f;
    public float duration = 0.3f;

    private Coroutine shakeRoutine;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void TriggerShake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float time = 0f;
        tmpText.ForceMeshUpdate();

        while (time < duration)
        {
            TMP_TextInfo textInfo = tmpText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int meshIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

                float offsetX = (Mathf.PerlinNoise(Time.time * speed + i, 0) - 0.5f) * 2f * magnitude;
                float offsetY = (Mathf.PerlinNoise(0, Time.time * speed + i) - 0.5f) * 2f * magnitude;

                Vector3 offset = new Vector3(offsetX, offsetY, 0);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            time += Time.deltaTime;
            yield return null;
        }

        tmpText.ForceMeshUpdate(); // reset text back to normal
        shakeRoutine = null;
    }
}


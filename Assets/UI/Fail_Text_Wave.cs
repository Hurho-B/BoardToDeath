using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WavyText : MonoBehaviour
{
    private TMP_Text tmpText;
    private Mesh mesh;
    private Vector3[] vertices;

    [Header("Wave Settings")]
    [SerializeField] private float amplitude = 5f;    // How high each letter moves
    [SerializeField] private float frequency = 3f;    // How fast the wave oscillates
    [SerializeField] private float waveSpacing = 0.3f; // How far apart letters are in the wave pattern

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        tmpText.ForceMeshUpdate();
        mesh = tmpText.mesh;
        vertices = mesh.vertices;

        TMP_TextInfo textInfo = tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[vertexIndex + j];
                float wave = Mathf.Sin(Time.time * frequency - i * waveSpacing) * amplitude;
                vertices[vertexIndex + j] = orig + new Vector3(0, wave, 0);
            }
        }

        mesh.vertices = vertices;
        tmpText.canvasRenderer.SetMesh(mesh);
    }
}


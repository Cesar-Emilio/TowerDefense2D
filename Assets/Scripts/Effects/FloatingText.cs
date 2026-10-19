using TMPro;
using UnityEngine;

// Texto en el mundo (no en el Canvas) que sube y se desvanece. Ej.: "+3" al eliminar un enemigo.
[RequireComponent(typeof(TextMeshPro))]
public class FloatingText : MonoBehaviour {
    private const float Duration = 0.7f;
    private const float RiseSpeed = 1.2f;

    private TextMeshPro text;
    private Color startColor;
    private float elapsed;

    public static void Spawn(string message, Vector3 position, Color color) {
        GameObject textObject = new GameObject("FloatingText");
        textObject.transform.position = position;

        // TextMeshPro (sin UGUI) se dibuja en el mundo, como un sprite
        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.text = message;
        text.fontSize = 3.5f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.sortingOrder = 10;
        text.rectTransform.sizeDelta = new Vector2(3f, 1f);

        textObject.AddComponent<FloatingText>();
    }

    private void Start() {
        text = GetComponent<TextMeshPro>();
        startColor = text.color;
    }

    private void Update() {
        elapsed += Time.deltaTime;
        transform.position += Vector3.up * RiseSpeed * Time.deltaTime;

        Color color = startColor;
        color.a = Mathf.Lerp(1f, 0f, elapsed / Duration);
        text.color = color;

        if (elapsed >= Duration) Destroy(gameObject);
    }
}

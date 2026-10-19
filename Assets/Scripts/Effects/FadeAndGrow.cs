using UnityEngine;

// Efecto de "pop": el sprite crece y se desvanece, y al terminar se destruye
[RequireComponent(typeof(SpriteRenderer))]
public class FadeAndGrow : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Vector3 startScale;
    private float growTo;
    private float duration;
    private float elapsed;

    // Crea el efecto reutilizando el sprite y el material de quien lo pide (un enemigo, por ejemplo)
    public static void Spawn(SpriteRenderer source, Vector3 position, Color color, float size,
                             float growTo = 1.8f, float duration = 0.2f) {
        GameObject effect = new GameObject("Pop");
        effect.transform.position = position;
        effect.transform.localScale = Vector3.one * size;

        SpriteRenderer renderer = effect.AddComponent<SpriteRenderer>();
        renderer.sprite = source.sprite;
        renderer.sharedMaterial = source.sharedMaterial;
        renderer.color = color;
        renderer.sortingOrder = source.sortingOrder + 1;

        FadeAndGrow fade = effect.AddComponent<FadeAndGrow>();
        fade.growTo = growTo;
        fade.duration = duration;
    }

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        startScale = transform.localScale;
    }

    private void Update() {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        transform.localScale = Vector3.Lerp(startScale, startScale * growTo, t);
        Color color = startColor;
        color.a = Mathf.Lerp(startColor.a, 0f, t);
        spriteRenderer.color = color;

        if (t >= 1f) Destroy(gameObject);
    }
}

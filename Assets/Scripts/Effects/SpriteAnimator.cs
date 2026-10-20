using UnityEngine;

// Animación por cuadros sencilla: cambia el sprite del SpriteRenderer según el tiempo.
// Más simple que un Animator para animaciones de 2-5 cuadros que solo se repiten.
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour {
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameRate = 4f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool randomStart = true; // que no todas las copias se muevan al mismo tiempo

    private SpriteRenderer spriteRenderer;
    private float time;

    public float Duration => frames == null || frameRate <= 0f ? 0f : frames.Length / frameRate;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Restart();
    }

    // Para objetos que cambian de animación al crearse (por ejemplo, el enemigo según sus datos)
    public void Play(Sprite[] newFrames, float newFrameRate) {
        frames = newFrames;
        frameRate = newFrameRate;
        Restart();
    }

    private void Restart() {
        time = randomStart && loop ? Random.Range(0f, Duration) : 0f;
        Apply();
    }

    private void Update() {
        time += Time.deltaTime;
        Apply();
    }

    private void Apply() {
        if (frames == null || frames.Length == 0 || frameRate <= 0f) return;

        int index = (int) (time * frameRate);
        index = loop ? index % frames.Length : Mathf.Min(index, frames.Length - 1);
        spriteRenderer.sprite = frames[index];
    }
}

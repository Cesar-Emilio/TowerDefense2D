using UnityEngine;

// Efecto temporal: se ve un instante y desaparece.
// Si tiene un SpriteAnimator, dura exactamente lo que dura su animación.
public class ExplosionEffect : MonoBehaviour {
    [SerializeField] private float duration = 0.2f;

    private void Start() {
        SpriteAnimator animator = GetComponent<SpriteAnimator>();
        float time = animator != null && animator.Duration > 0f ? animator.Duration : duration;
        Destroy(gameObject, time);
    }
}

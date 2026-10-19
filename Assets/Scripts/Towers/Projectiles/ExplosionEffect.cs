using UnityEngine;

// Efecto temporal: se ve un instante y desaparece
public class ExplosionEffect : MonoBehaviour {
    [SerializeField] private float duration = 0.2f;

    private void Start() {
        Destroy(gameObject, duration);
    }
}

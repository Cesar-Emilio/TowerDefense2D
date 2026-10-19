using UnityEngine;

// Sacude la cámara un instante. Se agrega sola a la cámara principal la primera vez que se usa.
public class CameraShake : MonoBehaviour {
    private Vector3 basePosition;
    private float timeLeft;
    private float magnitude;

    public static void Shake(float duration, float magnitude) {
        Camera camera = Camera.main;
        if (camera == null) return;

        CameraShake shake = camera.GetComponent<CameraShake>();
        if (shake == null) shake = camera.gameObject.AddComponent<CameraShake>();

        // Si ya estaba temblando se queda con la sacudida más fuerte
        shake.timeLeft = Mathf.Max(shake.timeLeft, duration);
        shake.magnitude = Mathf.Max(shake.magnitude, magnitude);
    }

    private void Awake() {
        basePosition = transform.position;
    }

    private void LateUpdate() {
        if (timeLeft <= 0f) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) {
            transform.position = basePosition;
            magnitude = 0f;
            return;
        }

        Vector2 offset = Random.insideUnitCircle * magnitude;
        transform.position = basePosition + new Vector3(offset.x, offset.y, 0f);
    }
}

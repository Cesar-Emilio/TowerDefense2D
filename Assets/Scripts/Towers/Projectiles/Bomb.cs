using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float hitRadius = 0.3f;
    [SerializeField] private GameObject explosionPrefab; // círculo que se ve un instante al explotar

    private Vector3 target;
    private int damage;
    private float shotSpeed;
    private float explosionRadius;
    
    public void Init(Vector3 targetPos, int damage, float shotSpeed, float explosionRadius) {
        this.target = targetPos;
        this.damage = damage;
        this.shotSpeed = shotSpeed;
        this.explosionRadius = explosionRadius;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, shotSpeed * Time.deltaTime);

        if (transform.position == target) {
            Explode();
            return;
        }

        foreach (Enemy enemy in Enemy.Active) {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= hitRadius) {
                Explode();
                return;
            }
        }
    }

    private void Explode() {
        List<Enemy> frameHits = new List<Enemy>();

        foreach (Enemy enemy in Enemy.Active) {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= explosionRadius) {
                frameHits.Add(enemy);
            }
        }

        foreach (Enemy enemy in frameHits) {
            enemy.TakeDamage(damage);
        }

        if (explosionPrefab != null) {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // El círculo mide 1 de diámetro, igual que el indicador de rango
            explosion.transform.localScale = Vector3.one * explosionRadius * 2f;
        }
        CameraShake.Shake(0.1f, 0.06f);

        Destroy(gameObject);
    }
}

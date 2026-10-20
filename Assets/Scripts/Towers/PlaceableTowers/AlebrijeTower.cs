using UnityEngine;

// Alebrije: dispara varias chispas en abanico hacia su objetivo
public class AlebrijeTower : Tower {
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int shots = 3;
    [SerializeField] private float spreadAngle = 16f; // grados entre una chispa y la siguiente

    protected override void Shoot(Enemy target)
    {
        Vector3 toTarget = target.transform.position - transform.position;
        float baseAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        for (int i = 0; i < shots; i++) {
            // Centra el abanico en el objetivo: con 3 disparos los ángulos son -16, 0 y +16
            float angle = (baseAngle + (i - (shots - 1) / 2f) * spreadAngle) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            // Init recibe un punto: se le da uno lejano en esa dirección
            projectile.Init(transform.position + direction * 10f, current.damage, current.shotSpeed, current.pierce);
        }
    }
}

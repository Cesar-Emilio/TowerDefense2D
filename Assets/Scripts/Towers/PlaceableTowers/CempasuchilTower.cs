using System.Collections.Generic;
using UnityEngine;

public class CempasuchilTower : Tower {
    [SerializeField] private GameObject pulsePrefab; // círculo de pétalos que se ve un instante

    // No dispara: Tower solo llama a Shoot cuando hay al menos un enemigo en rango.
    // Ignora al objetivo y suelta un pulso que afecta a todos los enemigos dentro del rango.
    protected override void Shoot(Enemy target)
    {
        // Juntar primero y aplicar después: el daño puede crear hijos dentro de Enemy.Active
        List<Enemy> inRange = new List<Enemy>();
        foreach (Enemy enemy in Enemy.Active) {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= current.range) {
                inRange.Add(enemy);
            }
        }

        foreach (Enemy enemy in inRange) {
            enemy.ApplySlow(current.slowStrength, current.slowDuration);
            if (current.damage > 0) enemy.TakeDamage(current.damage);
        }

        if (pulsePrefab != null) {
            GameObject pulse = Instantiate(pulsePrefab, transform.position, Quaternion.identity);
            pulse.transform.localScale = Vector3.one * current.range * 2f;
        }
    }
}

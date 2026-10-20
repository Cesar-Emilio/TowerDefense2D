using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float hitRadius = 0.35f;
    [SerializeField] private float lifetime = 2f;

    private HashSet<Enemy> enemiesHit = new HashSet<Enemy>();

    private Vector3 direction;
    private int damage;
    private int pierceLeft;

    public void Init(Vector3 targetPosition, int damage, float shotSpeed, int pierce) {
        direction = (targetPosition - transform.position).normalized;
        transform.right = direction; // el sprite apunta a la derecha: así queda mirando hacia donde vuela
        this.damage = damage;
        this.speed = shotSpeed;
        pierceLeft = pierce;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        List<Enemy> frameHits = new List<Enemy>();

        //buscar a qn pegarle
        foreach (Enemy enemy in Enemy.Active) {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= hitRadius) {
                if (!enemiesHit.Contains(enemy)) {
                    frameHits.Add(enemy);
                }
            }
        }

        //Más de un golpe si tiene hijos el enemigo
        foreach(Enemy enemy in frameHits) {
            if (pierceLeft <= 0) break;

            enemiesHit.Add(enemy);
            enemy.TakeDamage(damage);
            pierceLeft--;
        }

        if(pierceLeft <= 0) {
            Destroy(gameObject);
        }
    }
}

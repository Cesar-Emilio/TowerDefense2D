using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float hitRadius = 0.35f;
    [SerializeField] private float lifetime = 2f;

    private Vector3 direction;
    private int damage;

    public void Init(Vector3 targetPosition, int damage, float shotSpeed) {
        direction = (targetPosition - transform.position).normalized;
        this.damage = damage;
        this.speed = shotSpeed;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        //buscar a qn pegarle
        Enemy hit = null;
        foreach (Enemy enemy in Enemy.Active) {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= hitRadius) {
                hit = enemy;
                break;
            }
        }

        // ya sabiendo a qn, hacer el daño
        if (hit != null) {
            hit.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

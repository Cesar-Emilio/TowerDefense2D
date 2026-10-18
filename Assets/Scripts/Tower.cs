using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static readonly List<Tower> All = new List<Tower>();

    [SerializeField] private float range = 3f;
    [SerializeField] private float fireRate = 1.5f; // disparos por segundo
    [SerializeField] private int damage = 1;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int cost = 200;
    [SerializeField] private float radius = 0.35f;
    [SerializeField] private SpriteRenderer rangeIndicator;

    public int Cost => cost;
    public float Radius => radius;

    private float cooldown; // segundos que faltan para poder disparar

    private void Awake() {
        rangeIndicator.transform.localScale = Vector3.one * range * 2f;
        ShowRange(false);
    }

    private void Start() => All.Add(this);
    private void OnDestroy() => All.Remove(this);

    public void ShowRange(bool show) => rangeIndicator.gameObject.SetActive(show);
    public void SetRangeColor(Color color) => rangeIndicator.color = color;

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0f) return;

        Enemy target = FindTarget();
        if (target == null) return; // el cooldown queda en 0: dispara en cuanto entre alguien

        Shoot(target);
        cooldown = 1f / fireRate;
    }

    // "Primero": el que lleva más distancia recorrida dentro del rango
    private Enemy FindTarget()
    {
        Enemy best = null;
        foreach (Enemy enemy in Enemy.Active)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > range) continue;

            if (best == null || enemy.DistanceTravelled > best.DistanceTravelled)
                best = enemy;
        }
        return best;
    }

    private void Shoot(Enemy target)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Init(target.transform.position, damage);
    }

    // Dibuja el rango en la vista Scene cuando seleccionas la torre
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static readonly List<Tower> All = new List<Tower>();

    [SerializeField] private string towerName;
    [SerializeField] private float range = 3f;
    [SerializeField] private float fireRate = 1.5f; // disparos por segundo
    [SerializeField] private int damage = 1;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int cost = 200;
    [SerializeField] private float radius = 0.35f;
    [SerializeField] private SpriteRenderer rangeIndicator;
    [SerializeField, Range(0f, 1f)] private float sellRatio = 0.4f;

    private TargetMode targetMode = TargetMode.First; 
    private float totalSpent;

    public int Cost => cost;
    public float Radius => radius;
    public TargetMode TargetMode => targetMode;
    //TODO: Talvez crear una configuración para evitar estos numeros mágicos?
    public int SellValue => (int) (totalSpent * sellRatio);
    public string TowerName => towerName;

    private float cooldown; // segundos que faltan para poder disparar

    private void Awake() {
        totalSpent = (float) cost;
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

            if (best == null || IsBetter(enemy, best))
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

    private bool IsBetter(Enemy candidate, Enemy current) {
        switch (targetMode) {
            case TargetMode.First:
                return candidate.DistanceTravelled > current.DistanceTravelled;
            case TargetMode.Last:
                return candidate.DistanceTravelled < current.DistanceTravelled;
            case TargetMode.Strong:
                // Dos condiciones:
                //  1. Apunta al candidato si tiene más vida
                //  2. En caso de que no, si tienen la misma vida compara al que tenga una mayor distancia recorrida
                return candidate.Data.maxHealth > current.Data.maxHealth || 
                    (candidate.Data.maxHealth == current.Data.maxHealth &&
                    candidate.DistanceTravelled > current.DistanceTravelled);
            case TargetMode.Close:
                return Vector2.Distance(transform.position, candidate.transform.position) < Vector2.Distance(transform.position, current.transform.position);
            default:
                Debug.LogWarning($"Modo de objetivo desconocido: {targetMode}");
                return false;
        }
    }

    public void CycleTargetMode(bool next) {
        int sum = next ? 1 : -1;
        int enumLength = System.Enum.GetValues(typeof(TargetMode)).Length;
        targetMode = (TargetMode) (((int) targetMode + enumLength + sum) % enumLength);
    }
}
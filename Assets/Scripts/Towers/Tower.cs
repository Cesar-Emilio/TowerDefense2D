using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static readonly List<Tower> All = new List<Tower>();

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private SpriteRenderer rangeIndicator;
    [SerializeField, Range(0f, 1f)] private float sellRatio = 0.4f;
    [SerializeField] private TowerData data;

    private TargetMode targetMode = TargetMode.First; 
    private int[] levels;
    private TowerStats current;

    public TowerData TowerData => data;
    public TargetMode TargetMode => targetMode;
    public TowerStats CurrentStats => current;
    public string TowerName => data.towerName;
    public int Cost => data.cost;
    public float Radius => data.radius;

    public int TotalSpent { get; private set; }
    public int SellValue => (int) (TotalSpent * sellRatio);


    private float cooldown;

    private void Awake() {
        levels = new int[data.paths.Length];

        RecalculateStats();
        TotalSpent = data.cost;
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
        cooldown = 1f / current.rateOfFire;
    }

    // "Primero": el que lleva más distancia recorrida dentro del rango
    private Enemy FindTarget()
    {
        Enemy best = null;
        foreach (Enemy enemy in Enemy.Active)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > current.range) continue;

            if (best == null || IsBetter(enemy, best))
                best = enemy;
        }
        return best;
    }

    private void Shoot(Enemy target)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Init(target.transform.position, current.damage, current.shotSpeed);
    }

    // Dibuja el rango en la vista Scene cuando seleccionas la torre
    private void OnDrawGizmosSelected()
    {
        float range = (current != null) ? current.range : data.stats.range;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private bool IsBetter(Enemy candidate, Enemy currentEnemy) {
        switch (targetMode) {
            case TargetMode.First:
                return candidate.DistanceTravelled > currentEnemy.DistanceTravelled;
            case TargetMode.Last:
                return candidate.DistanceTravelled < currentEnemy.DistanceTravelled;
            case TargetMode.Strong:
                // Dos condiciones:
                //  1. Apunta al candidato si tiene más vida
                //  2. En caso de que no, si tienen la misma vida compara al que tenga una mayor distancia recorrida
                return candidate.Data.maxHealth > currentEnemy.Data.maxHealth || 
                    (candidate.Data.maxHealth == currentEnemy.Data.maxHealth &&
                    candidate.DistanceTravelled > currentEnemy.DistanceTravelled);
            case TargetMode.Close:
                return Vector2.Distance(transform.position, candidate.transform.position) < Vector2.Distance(transform.position, currentEnemy.transform.position);
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

    public void RecalculateStats() {
        // 1. Colocar las stats base
        current = new TowerStats{
            damage = data.stats.damage,
            range = data.stats.range,
            shotSpeed = data.stats.shotSpeed,
            rateOfFire = data.stats.rateOfFire,
            pierce = data.stats.pierce,
            seesCamo = data.stats.seesCamo
        };

        // 2. Colocar las mejoras
        for (int path = 0; path < data.paths.Length; path++) {
            for (int level = 0; level < levels[path]; level++) {
                ApplyUpgrade(current, data.paths[path].upgrades[level]);
            }
        }

        rangeIndicator.transform.localScale = Vector3.one * current.range * 2f;
    }

    private void ApplyUpgrade(TowerStats stats, UpgradeData upgrade) {
        stats.damage += upgrade.bonusDamage;
        stats.range += upgrade.bonusRange;
        stats.shotSpeed += upgrade.bonusShotSpeed;
        stats.rateOfFire = stats.rateOfFire + upgrade.fireRateMultiplier * stats.rateOfFire;
        stats.pierce += upgrade.bonusPierce;
        stats.seesCamo = stats.seesCamo ? stats.seesCamo : upgrade.grantsCamoDetection;
    }

    public bool CanUpgrade(int pathIndex) {
        // Solo revisar si existe una siguiente mejora en ese camino
        if(GetNextUpgrade(pathIndex) == null) return false; 

        // Revisar los límites de niveles
        // Si el siguiente nivel es 2 o más, revisar que ningún otro camino esté en 2 o más
        int siguienteNivel = levels[pathIndex] + 1;
        //TODO: talvez alguna config global para máximo de nivel?
        if (siguienteNivel < 2) return true;

        for (int i = 0; i < data.paths.Length; i++){
            if (i == pathIndex) continue;
            if (levels[i] >= 2) return false;
        }
        return true;
    }

    public bool TryUpgrade(int pathIndex) {
        if (!CanUpgrade(pathIndex)) return false;

        UpgradeData upgrade = GetNextUpgrade(pathIndex);

        if (!GameManager.Instance.TrySpend(upgrade.upgradeCost)) return false;

        levels[pathIndex]++;
        TotalSpent += upgrade.upgradeCost;
        RecalculateStats();
        return true;
    }

    // Devuelve la siguiente mejora de un camino
    public UpgradeData GetNextUpgrade(int pathIndex) {
        if (pathIndex >= data.paths.Length || pathIndex < 0) return null;

        if (levels[pathIndex] >= data.paths[pathIndex].upgrades.Count) return null;
        return data.paths[pathIndex].upgrades[levels[pathIndex]];
    }
}
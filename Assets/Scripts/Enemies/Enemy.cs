using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    // Una sola lista recorrida para buscar enemigos
    public static readonly List<Enemy> Active = new List<Enemy>();

    [SerializeField] private EnemyData data;

    [SerializeField] private Color slowTint = new Color(0.4f, 0.7f, 1f); // azulado mientras va lento
    [SerializeField, Range(0f, 1f)] private float maxSlowStrength = 0.9f;  // nunca se detiene por completo
    [SerializeField, Range(0f, 1f)] private float camoAlpha = 0.4f;        // camuflados: semitransparentes
    [SerializeField] private Color terrorAuraColor = new Color(0.6f, 0.2f, 0.8f, 0.15f);

    private int targetIndex = 1;
    private int health;
    private bool isDead;

    private SpriteRenderer spriteRenderer;
    private Color baseColor; // color de los datos, con el alfa del camuflaje ya aplicado
    private float slowMultiplier = 1f; // 1 = velocidad normal
    private float slowTimer;

    public float DistanceTravelled { get; private set; }
    public EnemyData Data => data;
    public bool IsCamo { get; private set; }

    public void Setup(EnemyData data, int targetIndex, float distanceTravelled, bool camo = false) {
        this.data = data;
        this.health = data.maxHealth;
        this.targetIndex = targetIndex;
        DistanceTravelled = distanceTravelled;
        IsCamo = camo;

        baseColor = data.color;
        if (camo) baseColor.a = camoAlpha;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = baseColor;
        transform.localScale = new Vector3(data.size, data.size, 1f);

        if (data.terrorRadius > 0f) CreateTerrorAura();
    }

    // Círculo que muestra hasta dónde llega el terror (la lógica la calcula cada Tower)
    private void CreateTerrorAura() {
        GameObject aura = new GameObject("TerrorAura");
        aura.transform.SetParent(transform, false);
        // El padre ya está escalado por data.size, así que se compensa para que mida el radio real
        aura.transform.localScale = Vector3.one * (data.terrorRadius * 2f / data.size);

        SpriteRenderer auraRenderer = aura.AddComponent<SpriteRenderer>();
        auraRenderer.sprite = spriteRenderer.sprite;
        auraRenderer.sharedMaterial = spriteRenderer.sharedMaterial;
        auraRenderer.color = terrorAuraColor;
        auraRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    // La lentitud no se acumula: se queda la más fuerte y el tiempo se renueva
    public void ApplySlow(float strength, float duration) {
        if (isDead || strength <= 0f || duration <= 0f) return;

        float multiplier = 1f - Mathf.Clamp(strength, 0f, maxSlowStrength);
        slowMultiplier = Mathf.Min(slowMultiplier, multiplier);
        slowTimer = Mathf.Max(slowTimer, duration);
        Color slowed = Color.Lerp(baseColor, slowTint, 0.5f);
        slowed.a = baseColor.a; // un camuflado lento sigue viéndose camuflado
        spriteRenderer.color = slowed;
    }

    private void UpdateSlow() {
        if (slowTimer <= 0f) return;

        slowTimer -= Time.deltaTime;
        if (slowTimer <= 0f) {
            slowMultiplier = 1f;
            spriteRenderer.color = baseColor;
        }
    }

    private void OnEnable() => Active.Add(this);
    private void OnDisable() => Active.Remove(this);

    private void Update()
    {
        UpdateSlow();

        Vector3 target = EnemyPath.Instance.GetWaypoint(targetIndex);
        Vector3 before = transform.position;

        transform.position = Vector3.MoveTowards(before, target, data.speed * slowMultiplier * Time.deltaTime);
        DistanceTravelled += Vector3.Distance(before, transform.position);

        if (transform.position == target) {
            targetIndex++;
            if (targetIndex >= EnemyPath.Instance.Count) 
                ReachEnd();
        }
    }

    public void TakeDamage(int amount) {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
            Die(-health);
    }

    private void Die(int extraDamage) {
        GameManager.Instance.AddMoney(data.reward);


        if (data.child != null && data.childCount > 0) {
            int damageToChilds = extraDamage / data.childCount;

            Vector3 lastTarget = EnemyPath.Instance.GetWaypoint(targetIndex - 1);
            Vector3 direction = (lastTarget - transform.position).normalized;
            for (int i = 0; i < data.childCount; i++) {
                float positionDelay = (float) i * 0.3f;


                Enemy child = EnemyFactory.Instance.Spawn(
                    data.child,
                    transform.position + direction * positionDelay,
                    targetIndex,
                    DistanceTravelled - positionDelay,
                    IsCamo // los hijos de un camuflado también salen camuflados
                );

                if (damageToChilds > 0) {
                    child.TakeDamage(damageToChilds);
                }
            }
        }
        RemoveFromGame();
    }

    private void ReachEnd() {
        GameManager.Instance.LoseLife(1);
        RemoveFromGame();
    }

    private void RemoveFromGame() {
        isDead = true;
        Active.Remove(this);
        Destroy(gameObject);
    }
}

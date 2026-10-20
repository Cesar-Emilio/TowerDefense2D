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
    [SerializeField] private float hitFlashDuration = 0.06f;
    [SerializeField] private Color hitTint = new Color(1f, 0.45f, 0.45f); // rojizo: el blanco no se nota sobre un sprite
    [SerializeField] private Sprite auraSprite; // círculo para el aura de terror (no el sprite del enemigo)
    [SerializeField] private Color rewardTextColor = new Color(1f, 0.85f, 0.3f);

    private int targetIndex = 1;
    private int health;
    private bool isDead;

    private SpriteRenderer spriteRenderer;
    private Color baseColor; // color de los datos, con el alfa del camuflaje ya aplicado
    private float slowMultiplier = 1f; // 1 = velocidad normal
    private float slowTimer;
    private float flashTimer;
    private float summonTimer;

    public float DistanceTravelled { get; private set; }
    public EnemyData Data => data;
    public bool IsCamo { get; private set; }
    public bool GivesReward { get; private set; } // los invocados por el jefe no dan dinero
    public int Health => health;

    public void Setup(EnemyData data, int targetIndex, float distanceTravelled, bool camo = false, bool givesReward = true) {
        this.data = data;
        this.health = data.maxHealth;
        this.targetIndex = targetIndex;
        DistanceTravelled = distanceTravelled;
        IsCamo = camo;
        GivesReward = givesReward;
        summonTimer = data.summonInterval;

        baseColor = data.color;
        if (camo) baseColor.a = camoAlpha;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = baseColor;
        transform.localScale = new Vector3(data.size, data.size, 1f);

        if (data.frames != null && data.frames.Length > 0) {
            SpriteAnimator animator = GetComponent<SpriteAnimator>();
            if (animator != null) animator.Play(data.frames, data.frameRate);
            else spriteRenderer.sprite = data.frames[0];
        }

        if (data.terrorRadius > 0f) CreateTerrorAura();
    }

    // Círculo que muestra hasta dónde llega el terror (la lógica la calcula cada Tower)
    private void CreateTerrorAura() {
        GameObject aura = new GameObject("TerrorAura");
        aura.transform.SetParent(transform, false);
        // El padre ya está escalado por data.size, así que se compensa para que mida el radio real
        aura.transform.localScale = Vector3.one * (data.terrorRadius * 2f / data.size);

        SpriteRenderer auraRenderer = aura.AddComponent<SpriteRenderer>();
        auraRenderer.sprite = auraSprite != null ? auraSprite : spriteRenderer.sprite;
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
    }

    private void UpdateTimers() {
        if (slowTimer > 0f) {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) slowMultiplier = 1f;
        }
        if (flashTimer > 0f) flashTimer -= Time.deltaTime;
    }

    // Un solo lugar decide el color: destello > lento > normal. El alfa del camuflaje siempre se respeta.
    private void RefreshColor() {
        Color color = baseColor;
        if (slowTimer > 0f) color = Color.Lerp(baseColor, slowTint, 0.5f);
        if (flashTimer > 0f) color = hitTint;
        color.a = baseColor.a;
        spriteRenderer.color = color;
    }

    private void OnEnable() => Active.Add(this);
    private void OnDisable() => Active.Remove(this);

    private void Update()
    {
        UpdateTimers();
        RefreshColor();
        UpdateSummon();

        Vector3 target = EnemyPath.Instance.GetWaypoint(targetIndex);
        Vector3 before = transform.position;

        transform.position = Vector3.MoveTowards(before, target, data.speed * slowMultiplier * Time.deltaTime);
        DistanceTravelled += Vector3.Distance(before, transform.position);

        // Mira hacia donde camina (en los tramos verticales conserva la última dirección)
        float dx = transform.position.x - before.x;
        if (Mathf.Abs(dx) > 0.0001f) spriteRenderer.flipX = dx < 0f;

        if (transform.position == target) {
            targetIndex++;
            if (targetIndex >= EnemyPath.Instance.Count)
                ReachEnd();
        }
    }

    // El jefe invoca enemigos a su alrededor cada cierto tiempo (no dan dinero)
    private void UpdateSummon() {
        if (data.summon == null || data.summonCount <= 0 || data.summonInterval <= 0f) return;

        summonTimer -= Time.deltaTime;
        if (summonTimer > 0f) return;
        summonTimer = data.summonInterval;

        // Empiezan una separación atrás para no aparecer encima del jefe
        SpawnBehind(data.summon, data.summonCount, 0.4f, 0.4f, 0, false);
    }

    public void TakeDamage(int amount) {
        if (isDead) return;

        health -= amount;
        flashTimer = hitFlashDuration;
        if (health <= 0)
            Die(-health);
    }

    private void Die(int extraDamage) {
        if (GivesReward) {
            GameManager.Instance.AddMoney(data.reward);
            FloatingText.Spawn($"+{data.reward}", transform.position, rewardTextColor);
        }
        FadeAndGrow.Spawn(spriteRenderer, transform.position, baseColor, data.size);
        if (data.isBoss) CameraShake.Shake(0.4f, 0.25f);

        if (data.child != null && data.childCount > 0) {
            // El daño sobrante se reparte entre los hijos (el residuo se pierde)
            int damageToChilds = extraDamage / data.childCount;
            SpawnBehind(data.child, data.childCount, 0.3f, 0f, damageToChilds, GivesReward);
        }
        RemoveFromGame();
    }

    // Crea enemigos sobre el camino, un poco detrás de este, con su mismo avance.
    // Los hijos de un camuflado también salen camuflados.
    private void SpawnBehind(EnemyData type, int count, float spacing, float firstOffset, int damage, bool givesReward) {
        Vector3 lastTarget = EnemyPath.Instance.GetWaypoint(targetIndex - 1);
        Vector3 direction = (lastTarget - transform.position).normalized;

        for (int i = 0; i < count; i++) {
            float positionDelay = firstOffset + i * spacing;

            Enemy spawned = EnemyFactory.Instance.Spawn(
                type,
                transform.position + direction * positionDelay,
                targetIndex,
                DistanceTravelled - positionDelay,
                IsCamo,
                givesReward
            );

            if (damage > 0) spawned.TakeDamage(damage);
        }
    }

    private void ReachEnd() {
        GameManager.Instance.LoseLife(data.livesCost);
        RemoveFromGame();
    }

    private void RemoveFromGame() {
        isDead = true;
        Active.Remove(this);
        Destroy(gameObject);
    }
}

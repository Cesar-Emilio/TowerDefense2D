using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Tower Defense/Enemy")]
public class EnemyData : ScriptableObject {
    public string displayName;
    public int maxHealth = 1;
    public float speed = 2f;
    public int reward = 1;

    [Header("Temporal design")]
    public Color color = Color.white;
    public float size = 0.5f;

    [Header("On death")]
    public EnemyData child; // Null = no drops
    public int childCount = 1;

    public int livesCost = 1; // vidas que quita al llegar al final

    [Header("Jefe")]
    public bool isBoss = false;          // muestra la barra de vida grande en el HUD
    public EnemyData summon;             // null = no invoca
    public int summonCount = 0;
    public float summonInterval = 0f;    // segundos entre invocaciones

    [Header("Terror (0 = sin aura)")]
    public float terrorRadius = 0f;
    [Range(0f, 1f)] public float terrorMultiplier = 1f; // 0.6 = las torres en el aura disparan al 60 %
}

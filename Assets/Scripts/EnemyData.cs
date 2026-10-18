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
}

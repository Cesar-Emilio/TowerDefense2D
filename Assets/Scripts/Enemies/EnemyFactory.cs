using UnityEngine;

public class EnemyFactory : MonoBehaviour {
    public static EnemyFactory Instance { get; private set; }
    
    [SerializeField] private Enemy prefab;

    private void Awake() {
        Instance = this;
    }

    public Enemy Spawn(EnemyData enemy, Vector3 pos, int waypoint, float distance, bool camo = false) {
        Enemy spawned = Instantiate(
            prefab,
            pos,
            Quaternion.identity
        );

        spawned.Setup(
            enemy,
            waypoint,
            distance,
            camo
        );

        return spawned;
    }
}

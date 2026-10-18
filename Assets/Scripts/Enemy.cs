using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    // Una sola lista recorrida para buscar enemigos
    public static readonly List<Enemy> Active = new List<Enemy>();

    [SerializeField] private EnemyData data;

    private int targetIndex = 1;
    private int health;
    private bool isDead;

    public float DistanceTravelled { get; private set; }
    public EnemyData Data => data;

    public void Setup(EnemyData data, int targetIndex, float distanceTravelled) {
        this.data = data;
        this.health = data.maxHealth;
        this.targetIndex = targetIndex;
        DistanceTravelled = distanceTravelled;

        gameObject.GetComponent<SpriteRenderer>().color = data.color;
        transform.localScale = new Vector3(data.size, data.size, 1f);
    }

    private void OnEnable() => Active.Add(this);
    private void OnDisable() => Active.Remove(this);

    private void Update()
    {
        Vector3 target = EnemyPath.Instance.GetWaypoint(targetIndex);
        Vector3 before = transform.position;

        transform.position = Vector3.MoveTowards(before, target, data.speed * Time.deltaTime);
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
                    DistanceTravelled - positionDelay
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

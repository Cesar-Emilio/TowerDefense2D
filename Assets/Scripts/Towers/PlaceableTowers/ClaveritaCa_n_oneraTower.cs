using UnityEngine;

public class ClaveritaCa_n_oneraTower : Tower {
    [SerializeField] private Bomb bombPrefab;

    protected override void Shoot(Enemy target)
    {
        // La bomba vuela al punto donde estaba el objetivo y explota en área
        Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        bomb.Init(target.transform.position, current.damage, current.shotSpeed, current.explosionRadius);
    }
}

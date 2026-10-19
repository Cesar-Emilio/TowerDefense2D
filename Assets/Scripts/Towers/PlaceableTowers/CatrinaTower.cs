using UnityEngine;

public class CatrinaTower : Tower
{
    [SerializeField] private Projectile projectilePrefab;

    protected override void Shoot(Enemy target)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Init(target.transform.position, current.damage, current.shotSpeed, current.pierce);
    }
}

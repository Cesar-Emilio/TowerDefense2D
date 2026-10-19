using UnityEngine;

[System.Serializable]
public class TowerStats {
    public int damage;
    public float range;
    public float explosionRadius;
    public float shotSpeed;
    public float rateOfFire;
    public int pierce;
    public bool seesCamo;
    public float slowStrength; // 0.35 = los enemigos van al 65 % de su velocidad
    public float slowDuration; // segundos
}

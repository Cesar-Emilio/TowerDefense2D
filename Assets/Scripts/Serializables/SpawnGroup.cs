using UnityEngine;

[System.Serializable]
public class SpawnGroup{
    public EnemyData enemy;
    public int count = 5;
    public float interval = 0.8f;
    public float startDelay = 0f;
    public bool camo = false; // cualquier enemigo puede salir camuflado
}

using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    
    public event System.Action OnWavesChanged;
    public event System.Action OnWaveCompleted; // al terminar cada oleada (la Ofrenda paga aquí)

    [SerializeField] private WaveData[] waves;

    public int CurrentWave { get; private set; }
    public int TotalWaves => waves.Length;
    public bool IsWaveRunning { get; private set; }

    private int groupsStillSpawning;

    private void Awake() => Instance = this;

    public void StartNextWave()
    {
        if (IsWaveRunning || CurrentWave >= waves.Length || GameManager.Instance.IsGameOver) return;

        WaveData wave = waves[CurrentWave];
        CurrentWave++;
        IsWaveRunning = true;
        groupsStillSpawning = wave.groups.Count;
        OnWavesChanged?.Invoke();

        foreach (SpawnGroup group in wave.groups)
            StartCoroutine(SpawnGroupRoutine(group));
    }

    // Una corrutina es un método que se puede "pausar": en cada yield return
    // Unity se sale y lo continúa en el punto donde se quedó cuando se cumple la espera.
    private IEnumerator SpawnGroupRoutine(SpawnGroup group)
    {
        yield return new WaitForSeconds(group.startDelay);

        for (int i = 0; i < group.count; i++)
        {
            EnemyFactory.Instance.Spawn(group.enemy, EnemyPath.Instance.GetWaypoint(0), 1, 0f, group.camo);
            yield return new WaitForSeconds(group.interval);
        }

        groupsStillSpawning--;
    }

    private void Update()
    {
        // TODO: la oleada termina cuando está corriendo, ya no hay grupos generando
        //       y no queda ningún enemigo vivo. Cuando pase:
        //   1. IsWaveRunning = false
        //   2. dar el completionBonus de la oleada que acaba de terminar
        //   3. si era la última → GameManager.Instance.Win()
        if (!IsWaveRunning) return;
        if (groupsStillSpawning > 0) return;
        if (Enemy.Active.Count > 0) return;

        IsWaveRunning = false;
        WaveData wave = waves[CurrentWave - 1];
        GameManager.Instance.AddMoney(wave.completionBonus);
        OnWaveCompleted?.Invoke();

        if (CurrentWave >= TotalWaves) GameManager.Instance.Win();
    }
}
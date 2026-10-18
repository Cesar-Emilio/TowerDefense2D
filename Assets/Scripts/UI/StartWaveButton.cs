using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartWaveButton : MonoBehaviour
{
    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => WaveManager.Instance.StartNextWave());
    }

    private void Update() {
        WaveManager waves = WaveManager.Instance;
        GameManager game = GameManager.Instance;
        button.interactable = !waves.IsWaveRunning && waves.CurrentWave < waves.TotalWaves && !game.IsGameOver;
    }
}

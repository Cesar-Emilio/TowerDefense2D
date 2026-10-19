using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Button))]
public class StartWaveButton : MonoBehaviour
{
    private Button button;

    [SerializeField] private Key startKeyShortcut = Key.Space;

    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => WaveManager.Instance.StartNextWave());
    }

    private void Update() {
        WaveManager waves = WaveManager.Instance;
        GameManager game = GameManager.Instance;

        bool interactable = !waves.IsWaveRunning && waves.CurrentWave < waves.TotalWaves && !game.IsGameOver;
        button.interactable = interactable;

        if (startKeyShortcut == Key.None || !Keyboard.current[startKeyShortcut].wasPressedThisFrame) return;
        if (game.IsGameOver || GameSpeed.IsPaused) return;

        // La misma tecla: sin oleada en curso la inicia; con oleada en curso alterna x1 / x2
        if (interactable) waves.StartNextWave();
        else if (waves.IsWaveRunning && GameSpeed.Instance != null) GameSpeed.Instance.ToggleFast();
    }
}

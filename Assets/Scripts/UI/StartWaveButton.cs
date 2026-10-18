using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Button))]
public class StartWaveButton : MonoBehaviour
{
    private Button button;

    [SerializeField] private Key startKeyShortcut;

    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => WaveManager.Instance.StartNextWave());
    }

    private void Update() {
        WaveManager waves = WaveManager.Instance;
        GameManager game = GameManager.Instance;

        bool interactable = !waves.IsWaveRunning && waves.CurrentWave < waves.TotalWaves && !game.IsGameOver;
        button.interactable = interactable;
        if (Keyboard.current[startKeyShortcut].wasPressedThisFrame && interactable) {
            WaveManager.Instance.StartNextWave();
        }
    }
}

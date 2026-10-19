using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Pausa, velocidad x1/x2 y la tecla Esc.
// Es el ÚNICO script que lee Esc, para que una sola pulsación no haga tres cosas a la vez.
public class GameSpeed : MonoBehaviour {
    public static GameSpeed Instance { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private float fastSpeed = 2f;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool paused;
    private bool fast;

    // static para poder preguntarlo desde cualquier lado sin revisar si Instance existe
    public static bool IsPaused => Instance != null && Instance.paused;

    private void Awake() {
        Instance = this;
        pausePanel.SetActive(false);
        ApplyTimeScale();
    }

    private void Update() {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (GameManager.Instance.IsGameOver) return;

        // Prioridad: quitar pausa > cancelar colocación > deseleccionar > pausar
        if (paused) Resume();
        else if (TowerPlacer.Instance.IsPlacing) TowerPlacer.Instance.CancelPlacing();
        else if (TowerSelection.Instance.Selected != null) TowerSelection.Instance.Deselect();
        else Pause();
    }

    public void TogglePause() {
        if (paused) Resume();
        else Pause();
    }

    public void Pause() {
        if (GameManager.Instance.IsGameOver) return;
        paused = true;
        pausePanel.SetActive(true);
        ApplyTimeScale();
    }

    public void Resume() {
        paused = false;
        pausePanel.SetActive(false);
        ApplyTimeScale(); // regresa a la velocidad que tenías (x1 o x2)
    }

    public void ToggleFast() {
        fast = !fast;
        ApplyTimeScale();
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f; // la escena nueva no debe empezar congelada ni acelerada
        SceneManager.LoadScene(mainMenuScene);
    }

    private void ApplyTimeScale() {
        if (speedLabel != null) speedLabel.text = fast ? "x2" : "x1";

        // Ganar o perder ya congela el juego; la velocidad no debe descongelarlo
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Time.timeScale = paused ? 0f : (fast ? fastSpeed : 1f);
    }
}

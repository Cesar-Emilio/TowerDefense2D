using System.Collections;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour {
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text wavesText;
    [SerializeField] private Color livesLostColor = new Color(1f, 0.25f, 0.25f);

    private int lastLives;
    private Color livesBaseColor;
    private Coroutine livesFlash;

    private void Start() {
        livesBaseColor = livesText.color;
        lastLives = GameManager.Instance.Lives;

        GameManager.Instance.OnStatsChanged += Refresh;
        WaveManager.Instance.OnWavesChanged += Refresh;
        Refresh();
    }

    private void OnDestroy() {
        GameManager.Instance.OnStatsChanged -= Refresh;
        WaveManager.Instance.OnWavesChanged -= Refresh;
    }

    private void Refresh(){
        livesText.text = $"Vidas: {GameManager.Instance.Lives}";
        moneyText.text = $"Pan de muerto: {GameManager.Instance.Money}";
        wavesText.text = $"Oleada {WaveManager.Instance.CurrentWave} / {WaveManager.Instance.TotalWaves}";

        if (GameManager.Instance.Lives < lastLives) {
            if (livesFlash != null) StopCoroutine(livesFlash);
            livesFlash = StartCoroutine(FlashLives());
        }
        lastLives = GameManager.Instance.Lives;
    }

    // Tiempo real (unscaled): al perder la última vida el juego se congela y el parpadeo debe terminar igual
    private IEnumerator FlashLives() {
        livesText.color = livesLostColor;
        yield return new WaitForSecondsRealtime(0.25f);
        livesText.color = livesBaseColor;
    }
}

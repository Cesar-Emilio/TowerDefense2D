using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour {
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text wavesText;

    private void Start() {
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
    }
}

using UnityEngine;

// Altar de ofrendas: no ataca. Al terminar cada oleada da pan de muerto (su estadística "income").
public class OfrendaTower : Tower {
    [SerializeField] private Color incomeTextColor = new Color(1f, 0.85f, 0.3f);

    protected override void Shoot(Enemy target) {
        // No ataca. Tower lo sigue llamando cuando hay enemigos cerca, y simplemente no hace nada.
    }

    // OnEnable/OnDisable: la vista previa de colocación tiene el script desactivado, así que nunca cobra
    private void OnEnable() {
        if (WaveManager.Instance != null) WaveManager.Instance.OnWaveCompleted += PayIncome;
    }

    private void OnDisable() {
        if (WaveManager.Instance != null) WaveManager.Instance.OnWaveCompleted -= PayIncome;
    }

    private void PayIncome() {
        if (current.income <= 0) return;
        GameManager.Instance.AddMoney(current.income);
        FloatingText.Spawn($"+{current.income}", transform.position + Vector3.up * 0.6f, incomeTextColor);
    }
}

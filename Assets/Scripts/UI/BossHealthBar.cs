using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Barra grande arriba de la pantalla: aparece mientras haya un jefe vivo
public class BossHealthBar : MonoBehaviour {
    [SerializeField] private GameObject content;
    [SerializeField] private Image fill;       // Image en modo "Filled" (horizontal)
    [SerializeField] private TMP_Text bossName;

    private Enemy boss;

    private void Awake() {
        content.SetActive(false);
    }

    private void Update() {
        // `!boss` es true si es null o si ya fue destruido (conversión de UnityEngine.Object)
        if (!boss) boss = FindBoss();

        content.SetActive(boss);
        if (!boss) return;

        bossName.text = boss.Data.displayName;
        fill.fillAmount = (float) boss.Health / boss.Data.maxHealth;
    }

    private static Enemy FindBoss() {
        foreach (Enemy enemy in Enemy.Active) {
            if (enemy.Data.isBoss) return enemy;
        }
        return null;
    }
}

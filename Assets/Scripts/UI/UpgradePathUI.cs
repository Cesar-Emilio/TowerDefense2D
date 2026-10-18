using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UpgradePathUI : MonoBehaviour {
    [SerializeField] private TMP_Text pathName;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeName;
    [SerializeField] private TMP_Text upgradeText;
    [SerializeField] private TMP_Text upgradeCost;

    private Tower tower;
    private int pathIndex;

    private void Awake() {
        upgradeButton.onClick.AddListener(() => tower.TryUpgrade(pathIndex));
    }

    public void Setup(Tower tower, int pathIndex) {
        this.tower = tower;
        this.pathIndex = pathIndex;
        pathName.text = tower.TowerData.paths[pathIndex].pathName;
    }

    private void Update() {
        if (!tower) return;

        UpgradeData upgrade = tower.GetNextUpgrade(pathIndex);
        bool canUpgrade = tower.CanUpgrade(pathIndex);

        upgradeButton.interactable = upgrade != null && canUpgrade && GameManager.Instance.Money >= upgrade.upgradeCost;

        //Si es el máximo nivel, solo coloca el botón de mejora en "Máximo" y deja la úlrima mejora
        if (upgrade == null) {
            upgradeCost.text = "Máximo";
            return;
        }
        
        if (!canUpgrade) {
            upgradeCost.text = "Bloqueado";
        } else {
            upgradeCost.text = upgrade.upgradeCost.ToString();
        }
        upgradeName.text = upgrade.upgradeName;
        upgradeText.text = $"{upgrade.description}\n{upgrade.GetDescription()}";
    }
}

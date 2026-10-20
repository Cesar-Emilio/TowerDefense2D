using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Button))]
public class ShopButton : MonoBehaviour {
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image icon; // opcional: el dibujo de la torre

    // Los asigna TowerShop al crear el botón
    private Tower towerPrefab;
    private Key shortcut;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        // Lambda: igual que en Java, pero con => en vez de ->
        button.onClick.AddListener(() => TowerPlacer.Instance.StartPlacing(towerPrefab));
    }

    public void Setup(Tower towerPrefab, Key shortcut) {
        this.towerPrefab = towerPrefab;
        this.shortcut = shortcut;

        string key = shortcut != Key.None ? $" [{shortcut}]" : "";
        label.text = $"{towerPrefab.TowerName}{key}\n{towerPrefab.Cost}";

        if (icon != null) {
            icon.sprite = towerPrefab.TowerData.icon;
            icon.enabled = icon.sprite != null;
            icon.preserveAspect = true;
        }
    }

    private void Update() {
        if (towerPrefab == null) return;

        // Gris si no te alcanza
        button.interactable = GameManager.Instance.Money >= towerPrefab.Cost;

        if (shortcut != Key.None && Keyboard.current[shortcut].wasPressedThisFrame) {
            TowerPlacer.Instance.StartPlacing(towerPrefab);
        }
    }
}

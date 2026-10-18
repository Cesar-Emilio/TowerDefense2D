using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Button))]
public class ShopButton : MonoBehaviour {
    [SerializeField] private Tower towerPrefab;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Key shortcut;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        // Lambda: igual que en Java, pero con => en vez de ->
        button.onClick.AddListener(() => TowerPlacer.Instance.StartPlacing(towerPrefab));
        label.text = $"{towerPrefab.name}\n{towerPrefab.Cost}";
    }

    private void Update() {
        // Gris si no te alcanza
        button.interactable = GameManager.Instance.Money >= towerPrefab.Cost;

        if (shortcut != Key.None && Keyboard.current[shortcut].wasPressedThisFrame) {
            TowerPlacer.Instance.StartPlacing(towerPrefab);
        }

    }
}

using UnityEngine;
using UnityEngine.InputSystem;

// Crea un botón de la tienda por cada torre de la lista.
// Agregar una torre nueva a la tienda = agregar su prefab a "Towers" en el Inspector.
public class TowerShop : MonoBehaviour {
    [SerializeField] private ShopButton buttonPrefab;
    [SerializeField] private Tower[] towers;
    [SerializeField] private Key[] shortcuts = { Key.Q, Key.W, Key.E, Key.R, Key.T };

    private void Awake() {
        for (int i = 0; i < towers.Length; i++) {
            Key shortcut = i < shortcuts.Length ? shortcuts[i] : Key.None;

            // Con el panel como padre, el Vertical Layout Group acomoda cada botón
            ShopButton button = Instantiate(buttonPrefab, transform);
            button.Setup(towers[i], shortcut);
        }
    }
}

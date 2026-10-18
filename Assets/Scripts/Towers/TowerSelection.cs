using UnityEngine.InputSystem;
using UnityEngine;

public class TowerSelection : MonoBehaviour {
    public static TowerSelection Instance { get; private set; }

    [SerializeField] TowerPanel towerPanel;

    public Tower Selected { get; private set; }

    private void Awake() {
        Instance = this;
        Selected = null;
    }

    private void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !TowerPlacer.Instance.IsPlacing) {
            Deselect();
        }

        // Si no se dio click, ya se está colocando algo o el mouse está sobre UI no se hace nada
        if (
            !Mouse.current.leftButton.wasPressedThisFrame ||
            TowerPlacer.Instance.IsPlacing ||
            TowerPlacer.IsPointerOverUI()
        ) return;


        Vector2 mouse = TowerPlacer.GetMouseWorldPosition();
        Tower tower = FindTowerAt(mouse);

        if(tower == null) {
            Deselect();
            return;
        }

        Select(tower);
    }

    public Tower FindTowerAt(Vector2 point) {
        for (int i = 0; i < Tower.All.Count; i++) {
            Vector2 towerPosition = Tower.All[i].transform.position;

            if (Vector2.Distance(point, towerPosition) < Tower.All[i].Radius) {
                return Tower.All[i];
            }
        }

        return null;
    }

    public void Select(Tower tower) {
        if (Selected != null) Deselect();

        Selected = tower;
        towerPanel.Show(tower);
        Selected.ShowRange(true);
        Selected.SetRangeColor(TowerPlacer.Instance.ValidColor);
    }

    public void Deselect() {
        if (Selected == null) return;
        Selected.ShowRange(false);
        towerPanel.Hide();
        Selected = null;
    }

    public void SellSelected() {
        if (Selected == null) return;
        Tower tower = Selected;

        //Dar dinero
        GameManager.Instance.AddMoney(tower.SellValue);

        //Deseleccionar torre
        Deselect();

        //Destruir la torre
        Destroy(tower.gameObject);
    }
}

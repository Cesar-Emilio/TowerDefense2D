using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour {
    public static TowerPlacer Instance { get; private set; }

    [SerializeField] private Color validColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.35f);

    private Tower prefab; //Lo que se coloca
    private Tower preview; //Copia que sigue al mouse

    public Color ValidColor => validColor;

    public bool IsPlacing => preview != null;

    private void Awake() => Instance = this;

    public void StartPlacing(Tower towerPrefab) {
        CancelPlacing();
        if (GameSpeed.IsPaused || GameManager.Instance.IsGameOver) return;
        if (GameManager.Instance.Money < towerPrefab.Cost) return;

        prefab = towerPrefab;
        preview = Instantiate(towerPrefab);
        preview.enabled = false;
        preview.ShowRange(true);
    }

    public void CancelPlacing() {
        if (preview != null) Destroy(preview.gameObject);
        preview = null;
        prefab = null;
    }

    private void Update() {
        if (!IsPlacing) return;
        if (GameSpeed.IsPaused) return;
        if (GameManager.Instance.IsGameOver) {
            CancelPlacing();
            return;
        }

        // Esc lo maneja GameSpeed (decide entre cancelar, deseleccionar o pausar)
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            CancelPlacing();
            return;
        }

        Vector2 mouse = GetMouseWorldPosition();
        preview.transform.position = mouse;

        bool valid = IsValidPosition(mouse, preview.Radius);
        preview.SetRangeColor(valid ? validColor : invalidColor);

        if (Mouse.current.leftButton.wasPressedThisFrame && valid && !IsPointerOverUI()) {
            if (GameManager.Instance.TrySpend(prefab.Cost))
                Instantiate(prefab, mouse, Quaternion.identity);
            CancelPlacing();
        }
    }

    public static Vector2 GetMouseWorldPosition() {
        Vector2 screen = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(screen);
    }

    public static bool IsPointerOverUI() => 
        EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    
    private bool IsValidPosition(Vector2 position, float radius) {
        if (EnemyPath.Instance.DistanceToPath(position) < EnemyPath.Instance.Width / 2f + radius)
            return false;
        
        foreach (Tower tower in Tower.All) {
            if (Vector2.Distance(position, tower.transform.position) < radius + tower.Radius)
                return false;
        }

        Vector2 viewport = Camera.main.WorldToViewportPoint(position);
        return viewport.x > 0f && viewport.x < 1f && viewport.y > 0f && viewport.y < 1f;
    }
}

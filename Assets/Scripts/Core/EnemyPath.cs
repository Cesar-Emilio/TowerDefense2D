using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnemyPath : MonoBehaviour
{
    //Singleton: Solo hay un camino y cualquier script lo encuentra con EnemyPath.Instance
    public static EnemyPath Instance {get; private set; }

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float width = 1f;

    [Header("Pixel art (opcional)")]
    [SerializeField] private Sprite pathTile;          // si se asigna, el camino se dibuja con tiles en vez de la línea
    [SerializeField] private int pathSortingOrder = -5;

    // => es una forma corta de escribir un getter o un método de una línea
    public int Count => waypoints.Length;
    public Vector3 GetWaypoint(int index) => waypoints[index].position;
    public float Width => width;

    private void Awake() {
        Instance = this;
        DrawPath();
    }

    private void DrawPath() {
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = waypoints.Length;
        line.startWidth = width;
        line.endWidth = width;
        
        for (int i = 0; i < waypoints.Length; i++)
            line.SetPosition(i, waypoints[i].position);

        if (pathTile != null) {
            line.enabled = false;
            DrawTiles(line.sharedMaterial);
        }
    }

    // Un SpriteRenderer en modo Tiled por tramo: el tile se repite a lo largo del tramo.
    // Cada tramo se alarga medio ancho en cada punta para que las esquinas queden cubiertas.
    private void DrawTiles(Material material) {
        for (int i = 0; i < waypoints.Length - 1; i++) {
            Vector3 a = waypoints[i].position;
            Vector3 b = waypoints[i + 1].position;
            Vector3 delta = b - a;
            bool horizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y);
            float length = delta.magnitude + width;

            GameObject segment = new GameObject($"PathTile {i}");
            segment.transform.SetParent(transform, false);
            segment.transform.position = (a + b) / 2f;

            SpriteRenderer tiles = segment.AddComponent<SpriteRenderer>();
            tiles.sprite = pathTile;
            if (material != null) tiles.sharedMaterial = material;
            tiles.drawMode = SpriteDrawMode.Tiled;
            tiles.size = horizontal ? new Vector2(length, width) : new Vector2(width, length);
            tiles.sortingOrder = pathSortingOrder;
        }
    }

    private void OnDrawGizmos() {
        if (waypoints == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++) {
            if (waypoints[i] == null || waypoints[i + 1] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }

    public float DistanceToPath(Vector2 point) {
        float min = float.MaxValue;
        for (int i = 0; i < waypoints.Length - 1; i++) {
            float d = DistanceToSegment(point, waypoints[i].position, waypoints[i + 1].position);
            if (d < min) min = d;
        }
        return min;
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b) {
        Vector2 ab = b - a;

        // t = que tan lejos sobre el tramo AB está el punto más cercano a P
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
        Vector2 closest = a + ab * t;
        return Vector2.Distance(p, closest);
    }
}

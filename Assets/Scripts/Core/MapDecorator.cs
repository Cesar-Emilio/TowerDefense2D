using System.Collections.Generic;
using UnityEngine;

// Dibuja el suelo con un tile repetido y reparte decoraciones al azar (siempre las mismas gracias a la semilla).
// Las decoraciones son solo visuales: no bloquean torres ni enemigos.
public class MapDecorator : MonoBehaviour {
    [SerializeField] private Sprite groundTile;
    [SerializeField] private Vector2 groundSize = new Vector2(26f, 16f);
    [SerializeField] private Sprite[] decorations;
    [SerializeField] private int decorationCount = 40;
    [SerializeField] private int seed = 1;
    [SerializeField] private float decorationScale = 1f;
    [SerializeField] private Material material;

    [Header("Dónde se pueden poner")]
    [SerializeField] private Rect area = new Rect(-10.4f, -5.7f, 18.4f, 10.4f); // deja libre la tienda (derecha)
    [SerializeField] private float pathMargin = 0.9f;
    [SerializeField] private float minDistance = 1.1f;

    private void Start() {
        if (groundTile != null) CreateGround();
        if (decorations != null && decorations.Length > 0) CreateDecorations();
    }

    private void CreateGround() {
        SpriteRenderer ground = CreateSprite("Ground", groundTile, Vector3.zero, -20);
        ground.drawMode = SpriteDrawMode.Tiled;
        ground.size = groundSize;
    }

    private void CreateDecorations() {
        System.Random random = new System.Random(seed);
        List<Vector2> placed = new List<Vector2>();
        EnemyPath path = EnemyPath.Instance; // null en el menú principal

        for (int tries = 0; tries < decorationCount * 15 && placed.Count < decorationCount; tries++) {
            Vector2 position = new Vector2(
                area.xMin + (float) random.NextDouble() * area.width,
                area.yMin + (float) random.NextDouble() * area.height);

            if (path != null && path.DistanceToPath(position) < path.Width / 2f + pathMargin) continue;
            if (placed.Exists(other => Vector2.Distance(other, position) < minDistance)) continue;

            placed.Add(position);
            Sprite sprite = decorations[random.Next(decorations.Length)];
            SpriteRenderer decoration = CreateSprite("Decoration", sprite, position, -3);
            decoration.flipX = random.NextDouble() < 0.5;
            decoration.transform.localScale = Vector3.one * decorationScale;
        }
    }

    private SpriteRenderer CreateSprite(string name, Sprite sprite, Vector3 position, int order) {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform, false);
        child.transform.position = position;

        SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        if (material != null) renderer.sharedMaterial = material;
        renderer.sortingOrder = order;
        return renderer;
    }
}

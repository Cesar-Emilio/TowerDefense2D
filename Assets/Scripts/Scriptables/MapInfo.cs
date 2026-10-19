using UnityEngine;

// Un mapa del menú de selección. Agregar un mapa = crear uno de estos y ponerlo en el MainMenu.
[CreateAssetMenu(fileName = "Map", menuName = "Tower Defense/Map")]
public class MapInfo : ScriptableObject {
    public string displayName;
    public string sceneName; // tiene que estar en Build Profiles → Scene List
}

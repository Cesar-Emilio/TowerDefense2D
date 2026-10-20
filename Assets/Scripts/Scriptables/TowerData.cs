using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Tower Defense/Tower")]
public class TowerData : ScriptableObject {
    public string towerName;
    public int cost;
    public float radius;
    public Sprite icon; // se muestra en el botón de la tienda
    
    [Header("Stats")]
    public TowerStats stats;

    [Header("Upgrades")]
    public UpgradePathData[] paths;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Tower Defense/Wave")]
public class WaveData : ScriptableObject{
    public List<SpawnGroup> groups = new List<SpawnGroup>();
    public int completionBonus = 100;
}

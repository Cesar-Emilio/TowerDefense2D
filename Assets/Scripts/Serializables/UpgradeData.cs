using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeData {
    public string upgradeName;
    public int upgradeCost;

    public int bonusDamage;
    public float bonusRange;
    public float bonusShotSpeed;
    public float fireRateMultiplier;
    public int bonusPierce;
    public bool grantsCamoDetection;

    [TextArea]
    public string description;

    // Arma la lista de efectos a partir de los números, para que nunca se desincronice del asset.
    // "+0.#;-0.#" pone el signo y quita el ".0" sobrante: 1 → "+1", 1.5 → "+1.5", -2 → "-2".
    public string GetDescription() {
        List<string> nonzero = new List<string>();

        if (bonusDamage != 0) nonzero.Add($"{bonusDamage:+0;-0} de daño");
        if (bonusRange != 0) nonzero.Add($"{bonusRange:+0.#;-0.#} de rango");
        if (bonusShotSpeed != 0) nonzero.Add($"{bonusShotSpeed:+0.#;-0.#} de velocidad de disparo");
        if (fireRateMultiplier != 0) nonzero.Add($"{fireRateMultiplier * 100:+0;-0} % de cadencia");
        if (bonusPierce != 0) nonzero.Add($"{bonusPierce:+0;-0} de perforación");
        if (grantsCamoDetection) nonzero.Add("Ve camuflados");

        return string.Join("\n", nonzero);
    }
}

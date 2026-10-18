using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TowerPanel : MonoBehaviour
{
    //Contenido, apagabla cuando no haya algo seleccionado
    [SerializeField] private GameObject content;

    //Valores de la torre
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text sellTowerText;
    [SerializeField] private Button sellTowerButton;

    //Modo de apuntado
    [SerializeField] private TMP_Text targetModeText;
    [SerializeField] private Button nextTargetModeButton;
    [SerializeField] private Button lastTargetModeButton;

    //Mejoras
    [SerializeField] private UpgradePathUI pathPrefab;
    [SerializeField] private Transform upgradesContainer; 
    
    private Tower tower;

    private void Awake()
    {
        // conectar el botón de objetivo con AddListener (como en ShopButton)
        // empezar oculto
        nextTargetModeButton.onClick.AddListener(() => tower.CycleTargetMode(true));
        lastTargetModeButton.onClick.AddListener(() => tower.CycleTargetMode(false));
        sellTowerButton.onClick.AddListener(() => TowerSelection.Instance.SellSelected());

        Hide();
    }

    public void Show(Tower tower)
    {
        // guardar la torre y prender `content`
        this.tower = tower;
        content.SetActive(true);

        foreach (Transform child in upgradesContainer) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < tower.TowerData.paths.Length; i++) {
            UpgradePathUI child = Instantiate(
                pathPrefab,
                upgradesContainer
            );

            child.Setup(tower, i);
        }
    }

    public void Hide()
    {
        // apagar `content` y olvidar la torre
        content.SetActive(false);
        tower = null;
    }

    private void Update() {
        // si no hay torre, salir
        // actualizar los textos: nombre y modo de objetivo actual
        if (tower == null) return;

        towerNameText.text = tower.TowerName;
        targetModeText.text = tower.TargetMode.ToString();
        sellTowerText.text = $"Vender: {tower.SellValue.ToString()}";
    }
}

using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text targetModeText;
    [SerializeField] private TMP_Text sellTowerText;
    [SerializeField] private Button nextTargetModeButton;
    [SerializeField] private Button lastTargetModeButton;
    [SerializeField] private Button sellTowerButton;
    
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
    }

    public void Hide()
    {
        // apagar `content` y olvidar la torre
        content.SetActive(false);
        tower = null;
    }

    private void Update()
    {
        // si no hay torre, salir
        // actualizar los textos: nombre y modo de objetivo actual
        if (tower == null) return;

        towerNameText.text = tower.TowerName;
        targetModeText.text = tower.TargetMode.ToString();
        sellTowerText.text = $"Vender: {tower.SellValue.ToString()}";
    }
}

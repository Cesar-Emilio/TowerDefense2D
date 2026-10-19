using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject mapPanel;

    [Header("Selección de mapa")]
    [SerializeField] private MapButton mapButtonPrefab;
    [SerializeField] private Transform mapsContainer;
    [SerializeField] private MapInfo[] maps;

    private void Awake() {
        Time.timeScale = 1f;

        // Un botón por mapa: agregar el mapa 2 es crear su MapInfo, no editar la interfaz
        foreach (MapInfo map in maps) {
            MapButton button = Instantiate(mapButtonPrefab, mapsContainer);
            button.Setup(map);
        }

        ShowMain();
    }

    public void ShowMain() {
        mainPanel.SetActive(true);
        mapPanel.SetActive(false);
    }

    public void ShowMaps() {
        mainPanel.SetActive(false);
        mapPanel.SetActive(true);
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Application.Quit no hace nada en el editor
#else
        Application.Quit();
#endif
    }
}

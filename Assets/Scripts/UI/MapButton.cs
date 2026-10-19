using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MapButton : MonoBehaviour {
    [SerializeField] private TMP_Text label;

    private MapInfo map;

    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene(map.sceneName);
        });
    }

    public void Setup(MapInfo map) {
        this.map = map;
        label.text = map.displayName;
    }
}

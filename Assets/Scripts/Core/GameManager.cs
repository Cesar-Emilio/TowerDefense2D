using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event System.Action OnStatsChanged;

    [SerializeField] private int startingLives = 20;
    [SerializeField] private int startingMoney = 650;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    public int Lives { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Money { get; private set; }

    private void Awake() {
        Instance = this;
        Lives = startingLives;
        Money = startingMoney;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void LoseLife(int amount) {
        if (IsGameOver) return;

        Lives = Mathf.Max(0, Lives - amount);
        OnStatsChanged?.Invoke();

        if (Lives == 0) {
            IsGameOver = true;
            losePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void AddMoney(int amount) {
        Money += amount;
        OnStatsChanged?.Invoke();
    }

    public bool TrySpend(int amount) {
        if (Money < amount) return false;

        Money -= amount;
        OnStatsChanged?.Invoke();
        return true;
    }

    public void Win() {
        if (IsGameOver) return;
        IsGameOver = true;

        winPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

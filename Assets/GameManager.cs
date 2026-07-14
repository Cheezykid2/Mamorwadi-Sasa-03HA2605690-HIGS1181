using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Turn Settings")]
    public bool isPlayerTurn = true;

    [Header("UI Elements")]
    public Text turnText;
    public Text playerHPText;
    public Text enemiesRemainingText;
    public GameObject winScreen;
    public GameObject loseScreen;

    private PlayerController player;
    private EnemyAI[] enemies;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);

        UpdateUI();
        ShowTurnText();
    }

    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        ShowTurnText();

        // Enemy turn
        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.TakeTurn();
                yield return new WaitForSeconds(0.5f); // small delay between enemies
            }
        }

        // Back to player turn
        isPlayerTurn = true;
        ShowTurnText();
    }

    public void UpdateUI()
    {
        if (player != null)
            playerHPText.text = "HP: " + player.playerHP;

        int aliveEnemies = 0;
        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null) aliveEnemies++;
        }
        enemiesRemainingText.text = "Enemies: " + aliveEnemies;
    }

    public void CheckWinCondition()
    {
        int aliveEnemies = 0;
        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null) aliveEnemies++;
        }

        if (aliveEnemies == 0)
        {
            GameOver(true);
        }
    }

    public void GameOver(bool playerWon)
    {
        if (playerWon)
        {
            winScreen.SetActive(true);
            Debug.Log("Player Wins!");
        }
        else
        {
            loseScreen.SetActive(true);
            Debug.Log("Player Lost!");
        }

        // Freeze game
        Time.timeScale = 0f;
    }

    private void ShowTurnText()
    {
        turnText.text = isPlayerTurn ? "Player Turn" : "Enemy Turn";
    }
}

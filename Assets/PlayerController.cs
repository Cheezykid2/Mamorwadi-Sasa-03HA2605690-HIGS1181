using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveTime = 0.2f;       // Time to move one tile
    public int playerHP = 5;            // Player health

    private Rigidbody2D rb2D;
    private Vector2 targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        targetPosition = rb2D.position;
    }

    private void Update()
    {
        // Only allow input if it's the player's turn and not already moving
        if (GameManager.instance.isPlayerTurn && !isMoving)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Vector2 moveDir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) moveDir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) moveDir = Vector2.right;

        if (moveDir != Vector2.zero)
        {
            targetPosition = rb2D.position + moveDir;
            StartCoroutine(MoveToTile(targetPosition));
        }
    }

    private System.Collections.IEnumerator MoveToTile(Vector2 destination)
    {
        isMoving = true;

        float elapsed = 0f;
        Vector2 startPos = rb2D.position;

        while (elapsed < moveTime)
        {
            rb2D.MovePosition(Vector2.Lerp(startPos, destination, elapsed / moveTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb2D.MovePosition(destination);
        isMoving = false;

        Debug.Log("Player moved to " + destination);

        // End player turn, switch to enemy turn
        GameManager.instance.EndPlayerTurn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            playerHP--;
            Debug.Log("Player hit! HP = " + playerHP);

            if (playerHP <= 0)
            {
                GameManager.instance.GameOver(false); // false = player lost
            }
        }
    }
}

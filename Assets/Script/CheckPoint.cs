using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static Checkpoint currentActiveCheckpoint;
    private SpriteRenderer sr;

    public GameObject healPromptUI;
    private bool playerInRange = false;
    private bool isActivated = false;
    private PlayerController player;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (healPromptUI != null)
            healPromptUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            if (player != null && player.currentHealth < player.maxHealth)
            {
                // Hồi máu
                player.currentHealth = player.maxHealth;
                player.healthBar.SetHealth(player.maxHealth);
                Debug.Log("Player healed at checkpoint!");

                isActivated = true;

                player.SetCheckpoint(transform.position);

                if (currentActiveCheckpoint != null && currentActiveCheckpoint != this)
                    currentActiveCheckpoint.ResetColor();

                sr.color = Color.green;
                currentActiveCheckpoint = this;

                if (healPromptUI != null)
                    healPromptUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<PlayerController>();
        if (player == null) return;

        playerInRange = true;

        if (!isActivated && healPromptUI != null)
            healPromptUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (healPromptUI != null)
            healPromptUI.SetActive(false);
    }

    private void ResetColor()
    {
        if (sr != null)
            sr.color = Color.white;
    }
}

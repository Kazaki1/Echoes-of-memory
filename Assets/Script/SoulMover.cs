using UnityEngine;

public class SoulMover : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("SoulMover: healthBar is NULL! Please assign it in the Inspector.");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogWarning("SoulMover: healthBar is NULL! Please assign it in the Inspector.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if (PlayerData.Instance != null)            
        {
            PlayerData.Instance.currentHealth -= 20;
            PlayerData.Instance.currentHealth = Mathf.Max(PlayerData.Instance.currentHealth, 0);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Lv1");
        Destroy(gameObject);
    }
}

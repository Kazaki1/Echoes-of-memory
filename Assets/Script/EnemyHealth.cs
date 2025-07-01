using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;
    public HealthBar healthBar;
    public bool isDead { get; private set; } = false;

    void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log($"{gameObject.name} đã chết!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lv1");
        Destroy(gameObject);
    }
}

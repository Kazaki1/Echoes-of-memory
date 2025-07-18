using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;
    public HealthBar healthBar;
    public bool isDead { get; private set; } = false;

    [Header("Điều kiện chuyển scene dựa trên Guilty Point")]
    public int guiltyThresholdA = 1; // Nếu GuiltyPoint < A → sceneA
    public int guiltyThresholdB = 3; // Nếu A <= GuiltyPoint < B → sceneB

    public string sceneA; // scene cho GuiltyPoint < A
    public string sceneB; // scene cho A <= GuiltyPoint < B
    public string sceneC; // scene cho GuiltyPoint >= B

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
        if (isDead) return;

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
        isDead = true;

        int guilty = RouteManager.Instance != null ? RouteManager.Instance.guiltyPoint : -1;
        string targetScene = "";

        if (guilty < guiltyThresholdA)
        {
            targetScene = sceneA;
        }
        else if (guilty >= guiltyThresholdA && guilty < guiltyThresholdB)
        {
            targetScene = sceneB;
        }
        else
        {
            targetScene = sceneC;
        }

        Debug.Log($"💀 Enemy chết. GuiltyPoint: {guilty} → Chuyển đến scene: {targetScene}");
        SceneManager.LoadScene(targetScene);
        Destroy(gameObject);
    }
}

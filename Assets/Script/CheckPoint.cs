using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static Checkpoint currentActiveCheckpoint; // Checkpoint được kích hoạt gần nhất
    private SpriteRenderer sr;
    private bool playerInRange = false; // Kiểm tra player có trong vùng checkpoint không
    private PlayerController nearbyPlayer; // Reference đến player gần checkpoint

    [Header("Visual Settings")]
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow; 
    public Color activeColor = Color.green; // Màu khi checkpoint được kích hoạt


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = defaultColor;

    }

    private void Update()
    {
        // Kiểm tra input khi player ở gần
        if (playerInRange && nearbyPlayer != null && Input.GetKeyDown(KeyCode.E))
        {
            ActivateCheckpoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Player vào vùng checkpoint
        playerInRange = true;
        nearbyPlayer = player;

        // Chỉ highlight nếu chưa được kích hoạt
        if (currentActiveCheckpoint != this)
        {
            sr.color = highlightColor;
        }
        Debug.Log("Press E to activate checkpoint");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        nearbyPlayer = null;

        if (currentActiveCheckpoint != this)
        {
            sr.color = defaultColor;
        }
    }

    private void ActivateCheckpoint()
    {
        if (nearbyPlayer == null) return;

        // Lưu checkpoint position
        nearbyPlayer.SetCheckpoint(transform.position);

        // Reset checkpoint cũ
        if (currentActiveCheckpoint != null && currentActiveCheckpoint != this)
        {
            currentActiveCheckpoint.ResetColor();
        }

        // Kích hoạt checkpoint mới
        sr.color = activeColor;
        currentActiveCheckpoint = this;

        Debug.Log("Checkpoint activated and saved!");

        // Có thể thêm hiệu ứng âm thanh hoặc particle ở đây
        PlayActivationEffect();
    }

    private void ResetColor()
    {
        if (sr != null)
            sr.color = defaultColor;
    }

    private void PlayActivationEffect()
    {
        StartCoroutine(ScaleEffect());
    }

    private System.Collections.IEnumerator ScaleEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // Scale up
        float elapsed = 0f;
        float duration = 0.1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
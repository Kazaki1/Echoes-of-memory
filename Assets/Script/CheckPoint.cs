using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    private static Checkpoint currentActiveCheckpoint;
    private SpriteRenderer sr;
    private bool playerInRange = false;
    private PlayerController nearbyPlayer;

    [Header("Visual Settings")]
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color activeColor = Color.green;

    [Header("Checkpoint Settings")]
    public bool isDefaultCheckpoint = false;

    // 🟢 THÊM BIẾN STATIC ĐỂ THEO DÕI VIỆC TELEPORT
    private static string lastTeleportedScene = "";

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = defaultColor;

        string currentScene = SceneManager.GetActiveScene().name;

        if (PlayerPrefs.HasKey("CheckpointScene"))
        {
            string savedScene = PlayerPrefs.GetString("CheckpointScene");
            float savedX = PlayerPrefs.GetFloat("CheckpointX");
            float savedY = PlayerPrefs.GetFloat("CheckpointY");

            if (savedScene == currentScene &&
                Vector2.Distance(new Vector2(savedX, savedY), (Vector2)transform.position) < 0.1f)
            {
                sr.color = activeColor;
                currentActiveCheckpoint = this;
            }

            // 🔥 Chỉ dịch chuyển nếu chưa teleport trong scene này
            if (lastTeleportedScene != currentScene)
            {
                StartCoroutine(WaitAndTeleportPlayer());
                lastTeleportedScene = currentScene;
                Debug.Log("🚀 Đã đánh dấu scene " + currentScene + " đã teleport");
            }
        }
        else if (isDefaultCheckpoint)
        {
            PlayerData.Instance?.SaveCheckpoint(transform.position);
            sr.color = activeColor;
            currentActiveCheckpoint = this;

            Debug.Log("⚡ Checkpoint mặc định được lưu: " + transform.position);

            // 🔥 Chỉ dịch chuyển nếu chưa teleport trong scene này
            if (lastTeleportedScene != currentScene)
            {
                StartCoroutine(WaitAndTeleportPlayer());
                lastTeleportedScene = currentScene;
                Debug.Log("🚀 Đã đánh dấu scene " + currentScene + " đã teleport (default)");
            }
        }
    }

    private void Update()
    {
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

        playerInRange = true;
        nearbyPlayer = player;

        if (currentActiveCheckpoint != this)
        {
            sr.color = highlightColor;
        }
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

        nearbyPlayer.SetCheckpoint(transform.position);

        if (currentActiveCheckpoint != null && currentActiveCheckpoint != this)
        {
            currentActiveCheckpoint.ResetColor();
        }

        sr.color = activeColor;
        currentActiveCheckpoint = this;

        PlayerData.Instance?.SaveCheckpoint(transform.position);

        Debug.Log("✅ Checkpoint được kích hoạt: " + transform.position);
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
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    public static void TeleportPlayerToCheckpointIfExists(GameObject playerObj)
    {
        if (PlayerData.Instance != null)
        {
            playerObj.transform.position = PlayerData.Instance.checkpointPosition;
            Debug.Log("🧍 Player đã dịch chuyển tới vị trí: " + playerObj.transform.position);
        }
    }

    public static Checkpoint FindDefaultCheckpoint()
    {
        Checkpoint[] all = GameObject.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        foreach (var cp in all)
        {
            if (cp.isDefaultCheckpoint)
                return cp;
        }
        return null;
    }

    // 🟢 THÊM HÀM ĐỂ RESET TRẠNG THÁI TELEPORT (gọi khi chuyển scene)
    public static void ResetTeleportState()
    {
        lastTeleportedScene = "";
        Debug.Log("🔄 Đã reset trạng thái teleport");
    }

    private System.Collections.IEnumerator WaitAndTeleportPlayer()
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = PlayerData.Instance.checkpointPosition;
            Debug.Log("🚀 Checkpoint.cs đã dịch chuyển Player đến: " + player.transform.position);
        }
    }
}
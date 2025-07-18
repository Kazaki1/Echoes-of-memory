using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderToBoss : MonoBehaviour
{
    [Header("Tên scene chứa trận Boss (phải nằm trong Build Settings)")]
    public string bossSceneName = "BossBattleScene";

    [Header("Tự động chuyển khi player chạm trigger?")]
    public bool autoTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (autoTrigger && other.CompareTag("Player"))
        {
            LoadBossScene();
        }
    }

    // Gọi hàm này nếu muốn chuyển scene bằng nút hoặc event khác
    public void LoadBossScene()
    {
        if (!string.IsNullOrEmpty(bossSceneName))
        {
            Debug.Log("Chuyển đến scene Boss: " + bossSceneName);
            SceneManager.LoadScene(bossSceneName);
        }
        else
        {
            Debug.LogWarning("Chưa đặt tên scene boss!");
        }
    }
}

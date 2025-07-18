using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerByGuiltyPoint : MonoBehaviour
{
    [Header("Guilty Point Thresholds")]
    public int guiltyThresholdA = 1;
    public int guiltyThresholdB = 3;

    [Header("Scene Mapping")]
    public string sceneA; // GuiltyPoint < A
    public string sceneB; // A <= GuiltyPoint < B
    public string sceneC; // GuiltyPoint >= B

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            int guilty = RouteManager.Instance != null ? RouteManager.Instance.guiltyPoint : -1;
            string targetScene = "";

            if (guilty < guiltyThresholdA)
                targetScene = sceneA;
            else if (guilty < guiltyThresholdB)
                targetScene = sceneB;
            else
                targetScene = sceneC;

            if (!string.IsNullOrEmpty(targetScene))
            {
                Debug.Log($"🚪 Player chạm Boss → GuiltyPoint: {guilty} → Chuyển Scene: {targetScene}");
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy scene phù hợp!");
            }
        }
    }
}

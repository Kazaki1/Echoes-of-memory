using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public PlayerController player; // Gán trong Inspector hoặc tìm tự động
    private int stepsToCombat;
    private int currentStepCount = 0;
    private bool inCombat = false;
    private bool pendingTransition = false;

    void Start()
    {
        ResetStepCounter();
    }

    // Gọi hàm này mỗi khi player di chuyển một bước
    public void OnPlayerStep()
    {
        if (inCombat) return;

        currentStepCount++;
        if (currentStepCount >= stepsToCombat)
        {
            TryStartCombatOrWait();
        }
    }

    void TryStartCombatOrWait()
    {
        if (player != null && player.Grounded())
        {
            StartCoroutine(CombatTransitionEffect());
        }
        else
        {
            pendingTransition = true;
        }
    }

    // Gọi hàm này từ PlayerController khi player vừa chạm đất
    public void NotifyPlayerGrounded()
    {
        if (pendingTransition && !inCombat)
        {
            pendingTransition = false;
            StartCoroutine(CombatTransitionEffect());
        }
    }

    IEnumerator CombatTransitionEffect()
    {
        inCombat = true;
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float duration = 1.0f; // tổng thời gian nhấp nháy
                float interval = 0.1f; // thời gian tắt/mở mỗi lần
                float timer = 0f;
                while (timer < duration)
                {
                    sr.enabled = !sr.enabled;
                    yield return new WaitForSeconds(interval);
                    timer += interval;
                }
                sr.enabled = true;
            }
        }
        StartCombat();
    }

    void StartCombat()
    {
        Debug.Log("Combat started! Switching scene...");
        // Random scene name
        string[] scenes = { "BattleScene1", "BattleScene2" };
        string chosenScene = scenes[Random.Range(0, scenes.Length)];
        SceneManager.LoadScene(chosenScene);
    }

    // Gọi hàm này khi combat kết thúc
    public void OnCombatEnd()
    {
        inCombat = false;
        pendingTransition = false;
        ResetStepCounter();
        Debug.Log("Combat ended! Step counter reset.");
    }

    void ResetStepCounter()
    {
        stepsToCombat = Random.Range(100,200); 
        currentStepCount = 0;
        Debug.Log($"Steps to next combat: {stepsToCombat}");
    }
}

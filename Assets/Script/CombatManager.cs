using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public PlayerController player;
    private int stepsToCombat;
    private int currentStepCount = 0;
    private bool inCombat = false;
    private bool pendingTransition = false;
    private System.Random trueRandom;

    [Header("Combat Scenes")]
    public string[] combatScenes; // ✅ Gán trong Inspector

    void Start()
    {
        trueRandom = new System.Random();
        ResetStepCounter();
    }

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
                float duration = 1.0f;
                float interval = 0.1f;
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
        if (combatScenes != null && combatScenes.Length > 0)
        {
            string chosenScene = combatScenes[Random.Range(0, combatScenes.Length)];
            SceneManager.LoadScene(chosenScene);
        }
        else
        {
            Debug.LogError("❌ Không có combat scene nào được gán trong Inspector!");
        }
    }

    public void OnCombatEnd()
    {
        inCombat = false;
        pendingTransition = false;
        ResetStepCounter();
        Debug.Log("Combat ended! Step counter reset.");
    }

    void ResetStepCounter()
    {
        stepsToCombat = trueRandom.Next(100, 200);
        currentStepCount = 0;
        Debug.Log($"Steps to next combat: {stepsToCombat}");
    }
}

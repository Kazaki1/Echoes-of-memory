using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public Image enemyImageUI;              // UI hiển thị ảnh enemy
    public EnemyData selectedEnemy;         // Enemy duy nhất, gán trong Inspector

    private EnemyAttackBase currentPattern;

    public void SetupEnemy()
    {
        if (selectedEnemy == null)
        {
            Debug.LogError("EnemyManager: selectedEnemy is not assigned!");
            return;
        }

        enemyImageUI.sprite = selectedEnemy.image;

        if (selectedEnemy.patternPrefabs == null || selectedEnemy.patternPrefabs.Length == 0)
        {
            Debug.LogError("EnemyManager: selectedEnemy has no patternPrefabs assigned!");
            return;
        }

        // Random pattern mỗi lần gọi
        int randomIndex = Random.Range(0, selectedEnemy.patternPrefabs.Length);
        GameObject patternPrefab = selectedEnemy.patternPrefabs[randomIndex];

        if (currentPattern != null)
            Destroy(currentPattern.gameObject);

        GameObject patternObj = Instantiate(patternPrefab);
        currentPattern = patternObj.GetComponent<EnemyAttackBase>();

        // Gán cho BattleManager nếu cần
        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.enemy = patternObj.GetComponent<EnemyController>();
        }
        else
        {
            Debug.LogError("EnemyManager: No BattleManager found in the scene!");
        }
    }
}
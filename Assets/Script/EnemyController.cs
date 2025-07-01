using UnityEngine;

public class EnemyController : MonoBehaviour
{


    [Header("Attack Script")]
    private EnemyAttackBase attackScript;

    void Awake()
    {
        attackScript = GetComponent<EnemyAttackBase>();
    }

    void Start()
    {

    }
    public void StartAttack()
    {
        if (attackScript == null)
            attackScript = GetComponent<EnemyAttackBase>();

        if (attackScript != null)
        {
            Debug.Log($"[EnemyController] StartAttack gọi {attackScript.GetType().Name}");
            attackScript.StartAttack();
        }
        else
        {
            Debug.LogError("[EnemyController] Không tìm thấy EnemyAttackBase trên prefab!");
        }
    }

    public void StopAttack()
    {
        if (attackScript == null)
            attackScript = GetComponent<EnemyAttackBase>();

        if (attackScript != null)
        {
            Debug.Log($"[EnemyController] StopAttack gọi {attackScript.GetType().Name}");
            attackScript.StopAttack();
        }
        else
        {
            Debug.LogError("[EnemyController] Không tìm thấy EnemyAttackBase khi StopAttack!");
        }
    }
}

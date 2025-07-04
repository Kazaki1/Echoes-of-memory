﻿using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    PlayerTurn,
    EnemyTurn,
    Busy
}

public class BattleManager : MonoBehaviour
{
    public GameObject soul;             // Player soul object
    public Button fightButton;          // Nút Fight
    public GameObject fightbar;         // GameObject chứa thanh Slider và FightbarController
    public EnemyController enemy;       // Script điều khiển quái vật
    public EnemyManager enemyManager;   // Thêm tham chiếu tới EnemyManager
    public EnemyHealth enemyHealth;
    public int playerAttackDamage = 100;

    private bool isFightBarActive = false;
    private FightbarController fightbarController;  // Reference đến FightbarController
    public BattleState state = BattleState.PlayerTurn;

    void Start()
    {
        // Khởi tạo UI cho lượt Player
        if (soul != null) soul.SetActive(false);
        if (fightbar != null)
        {
            fightbar.SetActive(false);
            // Lấy reference đến FightbarController
            fightbarController = fightbar.GetComponent<FightbarController>();

            // Subscribe to event từ FightbarController
            if (fightbarController != null)
            {
                fightbarController.OnPlayerStopFilling += OnPlayerStopFilling;
            }
        }

        if (fightButton != null)
        {
            fightButton.onClick.AddListener(OnPlayerChooseFight);
            fightButton.interactable = true;
        }

        StartPlayerTurn();
    }

    void Update()
    {

    }

    // -------------------------
    // PLAYER TURN
    // -------------------------
    void StartPlayerTurn()
    {
        Debug.Log("🔁 Player's Turn");
        state = BattleState.PlayerTurn;

        if (fightbar != null) fightbar.SetActive(false);
        if (fightButton != null) fightButton.interactable = true;
        if (soul != null) soul.SetActive(false);

        isFightBarActive = false;
    }

    public void OnPlayerChooseFight()
    {
        if (state != BattleState.PlayerTurn) return;

        Debug.Log("🗡️ Player chose FIGHT");

        state = BattleState.Busy;

        if (fightbar != null) fightbar.SetActive(true);
        if (fightButton != null) fightButton.interactable = false;

        isFightBarActive = true;
    }

    // Callback khi player dừng fightbar
    void OnPlayerStopFilling(float damageMultiplier)
    {
        Debug.Log($"💥 Player stops at multiplier: {damageMultiplier:P}");
        DealDamageToEnemy(damageMultiplier);
        EndPlayerTurn();
    }

    public void EndPlayerTurn()
    {
        Debug.Log("✅ Player ends turn");
        if (fightbar != null) fightbar.SetActive(false);
        isFightBarActive = false;

        if (soul != null) soul.SetActive(true);
        StartEnemyTurn();
    }

    void StartEnemyTurn()
    {
        Debug.Log("👾 Enemy's Turn");
        state = BattleState.EnemyTurn;

        // Random pattern mỗi lượt
        if (enemyManager != null)
        {
            Debug.Log("Gọi enemyManager.SetupEnemy()");
            enemyManager.SetupEnemy();
        }
        else
        {
            Debug.LogWarning("enemyManager bị null!");
        }

        if (enemy != null)
        {
            Debug.Log($"enemy hiện tại: {enemy.gameObject.name}");
            var attackScripts = enemy.GetComponents<EnemyAttackBase>();
            foreach (var attackScript in attackScripts)
            {
                var eventField = attackScript.GetType().GetField("OnAttackFinished");
                if (eventField != null && eventField.FieldType == typeof(System.Action))
                {
                    var current = (System.Action)eventField.GetValue(attackScript);
                    current -= OnEnemyAttackFinished;
                    current += OnEnemyAttackFinished;
                    eventField.SetValue(attackScript, current);
                }
            }
            enemy.StartAttack();
        }
        else
        {
            Debug.LogWarning("enemy bị null sau khi SetupEnemy!");
            Invoke(nameof(EndEnemyTurn), 3f);
        }
    }

    void EndEnemyTurn()
    {
        Debug.Log("💤 Enemy ends turn");

        if (enemy != null)
            enemy.StopAttack();

        if (soul != null) soul.SetActive(false);

        StartPlayerTurn();
    }

    void OnEnemyAttackFinished()
    {
        Debug.Log("Enemy attack finished. Back to player turn.");

        if (enemy != null)
            enemy.StopAttack();

        if (soul != null) soul.SetActive(false);

        StartPlayerTurn();
    }

    // Overload method với damage multiplier
    void DealDamageToEnemy(float damageMultiplier)
    {
        if (enemyHealth != null)
        {
            int finalDamage = Mathf.RoundToInt(playerAttackDamage * damageMultiplier);
            Debug.Log($"💥 Player deals {finalDamage} damage to enemy! (Base: {playerAttackDamage} × {damageMultiplier:P})");
            enemyHealth.TakeDamage(finalDamage);
        }
        else
        {
            Debug.LogWarning("⚠️ EnemyHealth is missing! Make sure it's assigned in the Inspector or via code.");
        }
    }

    // Method cũ để backward compatibility
    void DealDamageToEnemy()
    {
        DealDamageToEnemy(1f); // Full damage nếu không có multiplier
    }

    void OnDestroy()
    {
        // Unsubscribe để tránh memory leak
        if (fightbarController != null)
        {
            fightbarController.OnPlayerStopFilling -= OnPlayerStopFilling;
        }
    }
}
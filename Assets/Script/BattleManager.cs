using UnityEngine;
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
    public BattleState state = BattleState.PlayerTurn;

    void Start()
    {
        // Khởi tạo UI cho lượt Player
        if (soul != null) soul.SetActive(false);
        if (fightbar != null) fightbar.SetActive(false);

        if (fightButton != null)
        {
            fightButton.onClick.AddListener(OnPlayerChooseFight);
            fightButton.interactable = true;
        }

        StartPlayerTurn();
    }

    void Update()
    {
        if (isFightBarActive && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Player confirmed attack with Enter.");
            DealDamageToEnemy();  // ← Gây damage mỗi lần nhấn Enter
            EndPlayerTurn();      // ← Kết thúc lượt
        }
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
    void DealDamageToEnemy()
    {
        if (enemyHealth != null)
        {
            Debug.Log("💥 Player deals damage to enemy!");
            enemyHealth.TakeDamage(playerAttackDamage);
        }
        else
        {
            Debug.LogWarning("⚠️ EnemyHealth is missing! Make sure it's assigned in the Inspector or via code.");
        }
    }
}
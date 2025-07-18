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
    public GameObject soul;             
    public Button fightButton;
    public Button mercyButton;         
    public GameObject fightbar;         
    public EnemyController enemy;       
    public EnemyManager enemyManager;   
    public EnemyHealth enemyHealth;
    public int playerAttackDamage = 100;
    public string mercySceneName = "PeaceEnding";
    private bool hasAddedGuiltyPoint = false;
    private bool isFightBarActive = false;
    private FightbarController fightbarController; 
    public BattleState state = BattleState.PlayerTurn;

    void Start()
    {
        if (soul != null) soul.SetActive(false);
        if (fightbar != null)
        {
            fightbar.SetActive(false);
            fightbarController = fightbar.GetComponent<FightbarController>();

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
        if (mercyButton != null)
        {
            mercyButton.onClick.AddListener(OnPlayerChooseMercy);
            mercyButton.interactable = true;
        }
    }

    void Update()
    {

    }
    public void OnPlayerChooseMercy()
    {
        if (state != BattleState.PlayerTurn) return;

        Debug.Log("🤝 Player chose MERCY");
        state = BattleState.Busy;

        // (Có thể thêm hiệu ứng, fade, hoặc âm thanh tại đây nếu muốn)

        UnityEngine.SceneManagement.SceneManager.LoadScene(mercySceneName);
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
        if (mercyButton != null) mercyButton.gameObject.SetActive(false);
        if (state != BattleState.PlayerTurn) return;

        Debug.Log("🗡️ Player chose FIGHT");
        state = BattleState.Busy;

        if (fightbar != null) fightbar.SetActive(true);
        if (fightButton != null) fightButton.interactable = false;

        isFightBarActive = true;

        if (!hasAddedGuiltyPoint && RouteManager.Instance != null)
        {
            RouteManager.Instance.AddGuiltyPoint();
            hasAddedGuiltyPoint = true;
        }
    }
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
                if (attackScript is LaserAttackPattern laser)
                {
                    laser.OnAttackFinished -= OnEnemyAttackFinished;
                    laser.OnAttackFinished += OnEnemyAttackFinished;
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

    void DealDamageToEnemy()
    {
        DealDamageToEnemy(1f);
    }

    void OnDestroy()
    {
        if (fightbarController != null)
        {
            fightbarController.OnPlayerStopFilling -= OnPlayerStopFilling;
        }
    }
}
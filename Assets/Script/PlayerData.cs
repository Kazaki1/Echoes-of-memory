using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Position Data")]
    public Vector2 lastGroundedPosition;
    public Vector2 checkpointPosition;
    public Vector2 positionBeforeBattle;
    public string sceneBeforeBattle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;

        // Cập nhật checkpoint nếu đang ở đúng scene
        if (PlayerPrefs.HasKey("CheckpointScene"))
        {
            string savedScene = PlayerPrefs.GetString("CheckpointScene");
            if (savedScene == currentScene)
            {
                float x = PlayerPrefs.GetFloat("CheckpointX");
                float y = PlayerPrefs.GetFloat("CheckpointY");
                checkpointPosition = new Vector2(x, y);
                Debug.Log("🧭 PlayerData: Loaded checkpoint at " + checkpointPosition);
            }
            else
            {
                SetDefaultCheckpoint();
            }
        }
        else
        {
            SetDefaultCheckpoint();
        }

        // Nếu vừa thoát khỏi BattleScene → dịch chuyển về vị trí trước battle
        if (PlayerPrefs.GetInt("ReturnFromBattle", 0) == 1)
        {
            string returnSceneName = PlayerPrefs.GetString("BattleReturnScene", "");
            float px = PlayerPrefs.GetFloat("BattlePosX", 0);
            float py = PlayerPrefs.GetFloat("BattlePosY", 0);
            Vector2 savedPos = new Vector2(px, py);

            if (scene.name == returnSceneName)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = savedPos;
                    Debug.Log("🧍 Player trở lại vị trí sau BattleScene: " + savedPos);
                }

                PlayerPrefs.SetInt("ReturnFromBattle", 0);
                PlayerPrefs.Save();
            }
        }
    }

    private void SetDefaultCheckpoint()
    {
        Checkpoint defaultCP = Checkpoint.FindDefaultCheckpoint();
        if (defaultCP != null)
        {
            checkpointPosition = defaultCP.transform.position;
            Debug.Log("🧭 PlayerData: Loaded default checkpoint at " + checkpointPosition);
        }
        else
        {
            checkpointPosition = Vector2.zero;
            Debug.LogWarning("⚠️ PlayerData: No default checkpoint found.");
        }
    }

    public void SaveCheckpoint(Vector2 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
        PlayerPrefs.SetFloat("CheckpointX", newCheckpoint.x);
        PlayerPrefs.SetFloat("CheckpointY", newCheckpoint.y);
        PlayerPrefs.SetString("CheckpointScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        Debug.Log("✅ PlayerData: Saved checkpoint at " + newCheckpoint);
    }

    // Gọi trước khi vào BattleScene
    public void SavePositionBeforeBattle(Vector2 pos)
    {
        positionBeforeBattle = pos;
        sceneBeforeBattle = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetFloat("BattlePosX", pos.x);
        PlayerPrefs.SetFloat("BattlePosY", pos.y);
        PlayerPrefs.SetString("BattleReturnScene", sceneBeforeBattle);
        PlayerPrefs.SetInt("ReturnFromBattle", 1);
        PlayerPrefs.Save();

        Debug.Log("📌 PlayerData: Saved position before battle: " + pos + " in scene " + sceneBeforeBattle);
    }
}
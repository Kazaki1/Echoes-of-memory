using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    [System.Serializable]
    public class SaveData
    {
        public Vector2 playerPosition;
        public string currentScene;
        public int playerHealth;
        public float saveTime;
        public int guiltyPoint; // ✨ Thêm guilty point

        // Constructor cũ (để tương thích ngược)
        public SaveData(Vector2 pos, string scene, int health)
        {
            playerPosition = pos;
            currentScene = scene;
            playerHealth = health;
            saveTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // ✨ Sử dụng Unix timestamp
            guiltyPoint = 0; // Mặc định
        }

        // ✨ Constructor mới có guilty point
        public SaveData(Vector2 pos, string scene, int health, int guilty)
        {
            playerPosition = pos;
            currentScene = scene;
            playerHealth = health;
            saveTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // ✨ Sử dụng Unix timestamp
            guiltyPoint = guilty;
        }
    }

    [Header("Save Settings")]
    public bool autoSaveEnabled = true;
    public float autoSaveInterval = 30f; // Tự động save mỗi 30 giây

    private SaveData currentSaveData;
    private float lastAutoSaveTime;

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

    private void Start()
    {
        // Load dữ liệu save hiện tại (nếu có)
        LoadCurrentSaveData();
        lastAutoSaveTime = Time.time;
    }

    private void Update()
    {
        // Auto save
        if (autoSaveEnabled && Time.time - lastAutoSaveTime >= autoSaveInterval)
        {
            AutoSavePlayerData();
            lastAutoSaveTime = Time.time;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra nếu đang load game
        if (PlayerPrefs.GetInt("IsLoadingGame", 0) == 1)
        {
            StartCoroutine(LoadPlayerToSavedPosition());
            PlayerPrefs.SetInt("IsLoadingGame", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // ✨ Auto save khi vào scene mới (trừ scene battle)
            string sceneName = scene.name.ToLower();
            if (!sceneName.Contains("battle") && !sceneName.Contains("menu") && !sceneName.Contains("title"))
            {
                StartCoroutine(SaveAfterSceneLoad());
            }
        }
    }

    // ✨ COROUTINE SAVE SAU KHI LOAD SCENE
    private IEnumerator SaveAfterSceneLoad()
    {
        yield return new WaitForSeconds(1f); // Đợi scene load xong

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SavePlayerData();
            Debug.Log($"🎯 Auto save sau khi vào scene: {SceneManager.GetActiveScene().name}");
        }
    }

    // 🟢 LƯU DỮ LIỆU PLAYER
    public void SavePlayerData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("⚠️ Không tìm thấy Player để save!");
            return;
        }

        // ✨ Kiểm tra scene có được phép save không
        string currentSceneName = SceneManager.GetActiveScene().name.ToLower();
        if (currentSceneName.Contains("battle") || currentSceneName.Contains("menu") || currentSceneName.Contains("title"))
        {
            Debug.LogWarning($"⚠️ Không save trong scene: {SceneManager.GetActiveScene().name}");
            return;
        }

        // Lấy thông tin player
        Vector2 playerPos = player.transform.position;
        string currentScene = SceneManager.GetActiveScene().name;
        int playerHealth = 100; // Mặc định, có thể lấy từ PlayerController
        int currentGuiltyPoint = 0; // ✨ Mặc định guilty point

        // Lấy health từ PlayerData nếu có
        if (PlayerData.Instance != null)
        {
            playerHealth = PlayerData.Instance.currentHealth;
        }

        // ✨ Lấy guilty point từ RouteManager nếu có
        if (RouteManager.Instance != null)
        {
            currentGuiltyPoint = RouteManager.Instance.guiltyPoint;
        }

        // Tạo save data với guilty point
        currentSaveData = new SaveData(playerPos, currentScene, playerHealth, currentGuiltyPoint);

        // Lưu vào PlayerPrefs
        PlayerPrefs.SetFloat("SavedPlayerX", playerPos.x);
        PlayerPrefs.SetFloat("SavedPlayerY", playerPos.y);
        PlayerPrefs.SetString("SavedScene", currentScene);
        PlayerPrefs.SetInt("SavedHealth", playerHealth);
        PlayerPrefs.SetInt("SavedGuiltyPoint", currentGuiltyPoint); // ✨ Lưu guilty point
        PlayerPrefs.SetFloat("SaveTime", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()); // ✨ Lưu timestamp chính xác
        PlayerPrefs.SetInt("HasSaveData", 1);
        PlayerPrefs.Save();

        Debug.Log($"💾 Đã lưu dữ liệu Player tại: {playerPos} trong scene: {currentScene}");
        Debug.Log($"⚖️ Guilty Point đã lưu: {currentGuiltyPoint}"); // ✨ Log guilty point
    }

    // 🟢 LOAD DỮ LIỆU PLAYER
    public bool LoadPlayerData()
    {
        if (!HasSaveData())
        {
            Debug.LogWarning("⚠️ Không có dữ liệu save để load!");
            return false;
        }

        string savedScene = PlayerPrefs.GetString("SavedScene", "");

        if (string.IsNullOrEmpty(savedScene))
        {
            Debug.LogWarning("⚠️ Scene đã lưu không hợp lệ!");
            return false;
        }

        // Đánh dấu đang load game
        PlayerPrefs.SetInt("IsLoadingGame", 1);
        PlayerPrefs.Save();

        Debug.Log("🎮 Đang load game từ scene: " + savedScene);

        // Load scene
        SceneManager.LoadScene(savedScene);
        return true;
    }

    // 🟢 KIỂM TRA CÓ SAVE DATA KHÔNG
    public bool HasSaveData()
    {
        return PlayerPrefs.GetInt("HasSaveData", 0) == 1 &&
               PlayerPrefs.HasKey("SavedPlayerX") &&
               PlayerPrefs.HasKey("SavedPlayerY") &&
               PlayerPrefs.HasKey("SavedScene");
    }

    // 🟢 XÓA TẤT CẢ SAVE DATA
    public void ClearAllSaveData()
    {
        // Xóa save data
        PlayerPrefs.DeleteKey("SavedPlayerX");
        PlayerPrefs.DeleteKey("SavedPlayerY");
        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.DeleteKey("SavedHealth");
        PlayerPrefs.DeleteKey("SavedGuiltyPoint"); // ✨ Xóa guilty point
        PlayerPrefs.DeleteKey("SaveTime");
        PlayerPrefs.DeleteKey("HasSaveData");
        PlayerPrefs.DeleteKey("IsLoadingGame");

        // Xóa checkpoint data (nếu muốn)
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointScene");

        // Xóa battle data
        PlayerPrefs.DeleteKey("BattlePosX");
        PlayerPrefs.DeleteKey("BattlePosY");
        PlayerPrefs.DeleteKey("BattleReturnScene");
        PlayerPrefs.DeleteKey("ReturnFromBattle");

        PlayerPrefs.Save();

        currentSaveData = null;

        Debug.Log("🗑️ Đã xóa toàn bộ Save Data!");
    }

    // 🟢 LẤY THÔNG TIN SAVE DATA
    public SaveData GetSaveData()
    {
        if (!HasSaveData())
            return null;

        float x = PlayerPrefs.GetFloat("SavedPlayerX");
        float y = PlayerPrefs.GetFloat("SavedPlayerY");
        string scene = PlayerPrefs.GetString("SavedScene");
        int health = PlayerPrefs.GetInt("SavedHealth", 100);
        int guiltyPoint = PlayerPrefs.GetInt("SavedGuiltyPoint", 0); // ✨ Lấy guilty point

        return new SaveData(new Vector2(x, y), scene, health, guiltyPoint);
    }

    // 🟢 AUTO SAVE
    private void AutoSavePlayerData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SavePlayerData();
            Debug.Log("⏰ Auto Save thực hiện tại: " + System.DateTime.Now.ToString("HH:mm:ss"));
        }
    }

    // 🟢 LOAD DỮ LIỆU HIỆN TẠI
    private void LoadCurrentSaveData()
    {
        if (HasSaveData())
        {
            currentSaveData = GetSaveData();
            Debug.Log("📂 Đã load save data: " + currentSaveData.currentScene + " - " + currentSaveData.playerPosition);
            Debug.Log($"⚖️ Guilty Point: {currentSaveData.guiltyPoint}"); // ✨ Log guilty point
        }
    }

    // 🟢 COROUTINE LOAD PLAYER ĐẾN VỊ TRÍ ĐÃ LƯU
    private IEnumerator LoadPlayerToSavedPosition()
    {
        yield return null; // Đợi 1 frame

        if (!HasSaveData())
            yield break;

        SaveData saveData = GetSaveData();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = saveData.playerPosition;

            // Restore health nếu có PlayerData
            if (PlayerData.Instance != null)
            {
                PlayerData.Instance.currentHealth = saveData.playerHealth;
            }

            // ✨ Restore guilty point nếu có RouteManager
            if (RouteManager.Instance != null)
            {
                RouteManager.Instance.guiltyPoint = saveData.guiltyPoint;
            }

            Debug.Log("🚀 Player đã được load đến vị trí: " + saveData.playerPosition);
            Debug.Log("❤️ Health restored: " + saveData.playerHealth);
            Debug.Log($"⚖️ Guilty Point restored: {saveData.guiltyPoint}"); // ✨ Log guilty point restore
        }
    }

    // 🟢 LẤY THỜI GIAN SAVE CUỐI CÙNG
    public string GetLastSaveTime()
    {
        if (!HasSaveData())
            return "Chưa có save data";

        float saveTime = PlayerPrefs.GetFloat("SaveTime", 0);
        // ✨ Fix: Sử dụng UnixTimeStampToDateTime thay vì FromBinary
        System.DateTime startTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        System.DateTime dateTime = startTime.AddSeconds(saveTime).ToLocalTime();
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    // ✨ THÊM METHOD ĐỂ LẤY GUILTY POINT TỪ SAVE DATA
    public int GetSavedGuiltyPoint()
    {
        if (!HasSaveData())
            return 0;

        return PlayerPrefs.GetInt("SavedGuiltyPoint", 0);
    }

    // ✨ METHOD LẤY DỮ LIỆU CUỐI CÙNG CHI TIẾT
    public void LogLatestSaveData()
    {
        if (!HasSaveData())
        {
            Debug.Log("📝 Không có save data");
            return;
        }

        SaveData data = GetSaveData();
        Debug.Log("=== DỮ LIỆU SAVE CUỐI CÙNG ===");
        Debug.Log($"🎯 Scene: {data.currentScene}");
        Debug.Log($"📍 Vị trí: {data.playerPosition}");
        Debug.Log($"❤️ Health: {data.playerHealth}");
        Debug.Log($"⚖️ Guilty Point: {data.guiltyPoint}");
        Debug.Log($"⏰ Thời gian: {GetLastSaveTime()}");
        Debug.Log("===============================");
    }

    // ✨ METHOD FORCE SAVE (Bắt buộc save ngay lập tức)
    public void ForceSave()
    {
        Debug.Log("🔥 FORCE SAVE - Lưu dữ liệu ngay lập tức");
        SavePlayerData();
        LogLatestSaveData();
    }

    // ✨ SAVE TRƯỚC KHI VÀO BATTLE (Gọi từ code khác)
    public void SaveBeforeBattle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Lưu vị trí trước battle
        Vector2 playerPos = player.transform.position;
        string currentScene = SceneManager.GetActiveScene().name;

        // Lưu dữ liệu hiện tại trước khi vào battle
        SavePlayerData();

        Debug.Log($"⚔️ Đã save trước khi vào battle tại: {playerPos} - Scene: {currentScene}");
    }

    // ✨ SAVE SAU KHI THẮNG BOSS (Gọi từ code khác)
    public void SaveAfterBossVictory()
    {
        // Đợi một chút để chắc chắn scene đã load xong
        StartCoroutine(SaveAfterBossVictoryCoroutine());
    }

    private IEnumerator SaveAfterBossVictoryCoroutine()
    {
        yield return new WaitForSeconds(1f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SavePlayerData();
            Debug.Log("🏆 Đã save sau khi thắng boss!");
        }
    }

    // ✨ METHOD LẤY VỊ TRÍ SAVE CUỐI CÙNG
    public Vector2 GetLatestSavedPosition()
    {
        if (!HasSaveData())
            return Vector2.zero;

        return new Vector2(
            PlayerPrefs.GetFloat("SavedPlayerX", 0),
            PlayerPrefs.GetFloat("SavedPlayerY", 0)
        );
    }

    public string GetLatestSavedScene()
    {
        if (!HasSaveData())
            return "";

        return PlayerPrefs.GetString("SavedScene", "");
    }
}
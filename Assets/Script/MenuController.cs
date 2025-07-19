using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnStartGameClicked()
    {
        // 🔥 XÓA TẤT CẢ PLAYERDATA KHI BẮT ĐẦU GAME MỚI
        ClearAllPlayerData();

        SceneManager.LoadScene("Scene 1");
    }

    public void OnLoadGameClicked()
    {
        // Sử dụng SaveLoadManager thay vì PlayerData
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.LoadPlayerData();
        }
        else
        {
            Debug.LogWarning("⚠️ SaveLoadManager không tồn tại!");
        }
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Thoát game");
    }

    // 🟢 HÀM XÓA TẤT CẢ SAVE DATA
    private void ClearAllPlayerData()
    {
        // Sử dụng SaveLoadManager để xóa data
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.ClearAllSaveData();
        }
        else
        {
            // Fallback: xóa trực tiếp PlayerPrefs
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("🗑️ Đã xóa toàn bộ PlayerPrefs để bắt đầu game mới!");
        }

        // Reset PlayerData instance nếu có
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.checkpointPosition = Vector2.zero;
            PlayerData.Instance.lastGroundedPosition = Vector2.zero;
            PlayerData.Instance.positionBeforeBattle = Vector2.zero;
            PlayerData.Instance.sceneBeforeBattle = "";
            PlayerData.Instance.currentHealth = PlayerData.Instance.maxHealth;
        }
    }

    // 🟢 KIỂM TRA CÓ SAVE DATA KHÔNG (SỬ DỤNG SAVELOADMANAGER)
    private bool HasSaveData()
    {
        if (SaveLoadManager.Instance != null)
        {
            return SaveLoadManager.Instance.HasSaveData();
        }

        // Fallback check
        return PlayerPrefs.GetInt("HasSaveData", 0) == 1;
    }

}
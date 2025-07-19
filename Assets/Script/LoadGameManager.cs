using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Button loadButton;

    [Header("Load Settings")]
    public bool useCheckpointPosition = true; // Dùng checkpoint hay lastGroundedPosition

    private void Start()
    {
        // Gán sự kiện cho nút Load
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadGame);
        }

        // Kiểm tra xem có dữ liệu save không để enable/disable nút
        UpdateLoadButtonState();
    }

    public void LoadGame()
    {
        // Kiểm tra xem có dữ liệu save không
        if (!HasSaveData())
        {
            Debug.LogWarning("⚠️ Không có dữ liệu save để load!");
            return;
        }

        // Lấy scene cuối cùng đã lưu
        string savedScene = GetLastSavedScene();

        if (string.IsNullOrEmpty(savedScene))
        {
            Debug.LogWarning("⚠️ Không tìm thấy scene đã lưu!");
            return;
        }

        // Đánh dấu đang load game
        PlayerPrefs.SetInt("IsLoadingGame", 1);
        PlayerPrefs.Save();

        Debug.Log("🎮 Đang load game từ scene: " + savedScene);

        // Load scene
        SceneManager.LoadScene(savedScene);
    }

    private bool HasSaveData()
    {
        // Kiểm tra xem có checkpoint đã lưu không
        bool hasCheckpoint = PlayerPrefs.HasKey("CheckpointScene") &&
                           PlayerPrefs.HasKey("CheckpointX") &&
                           PlayerPrefs.HasKey("CheckpointY");

        // Hoặc kiểm tra lastGroundedPosition (nếu bạn lưu nó)
        bool hasLastGrounded = PlayerPrefs.HasKey("LastGroundedX") &&
                              PlayerPrefs.HasKey("LastGroundedY") &&
                              PlayerPrefs.HasKey("LastGroundedScene");

        return hasCheckpoint || hasLastGrounded;
    }

    private string GetLastSavedScene()
    {
        // Ưu tiên checkpoint trước
        if (PlayerPrefs.HasKey("CheckpointScene"))
        {
            return PlayerPrefs.GetString("CheckpointScene");
        }

        // Fallback về lastGroundedPosition scene
        if (PlayerPrefs.HasKey("LastGroundedScene"))
        {
            return PlayerPrefs.GetString("LastGroundedScene");
        }

        return "";
    }

    private void UpdateLoadButtonState()
    {
        if (loadButton != null)
        {
            loadButton.interactable = HasSaveData();

            // Tùy chọn: Thay đổi màu nút
            if (HasSaveData())
            {
                loadButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                loadButton.GetComponent<Image>().color = Color.gray;
            }
        }
    }

    // Gọi hàm này để refresh trạng thái nút (ví dụ khi có save mới)
    public void RefreshLoadButton()
    {
        UpdateLoadButtonState();
    }
}
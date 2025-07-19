using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIController : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResumeClicked()
    {
        TogglePauseMenu(); // Tắt menu, resume game
    }

    public void OnQuitToMenuClicked()
    {
        Time.timeScale = 1f; // Khôi phục lại tốc độ bình thường trước khi thoát
        SceneManager.LoadScene("Menu"); // Chuyển về scene Menu
    }
}

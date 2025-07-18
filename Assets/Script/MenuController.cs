using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnStartGameClicked()
    {
        SceneManager.LoadScene("Scene 1");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Thoát game");
    }
}

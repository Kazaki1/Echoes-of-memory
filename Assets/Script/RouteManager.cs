using UnityEngine;

public class RouteManager : MonoBehaviour
{
    public static RouteManager Instance;

    public int guiltyPoint = 0;
    public int sparedEnemies = 0;
    public string currentRoute = "Neutral";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}

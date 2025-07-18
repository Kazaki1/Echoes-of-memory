using UnityEngine;

public class RouteManager : MonoBehaviour
{
    public static RouteManager Instance { get; private set; }

    public int guiltyPoint = 0;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddGuiltyPoint(int amount = 1)
    {
        guiltyPoint += amount;
        Debug.Log($"⚖️ Guilty Point increased: {guiltyPoint}");
    }
}

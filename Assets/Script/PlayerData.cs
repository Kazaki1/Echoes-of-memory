using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    public int maxHealth;
    public int currentHealth;
    public Vector2 lastGroundedPosition;
    public Vector2 checkpointPosition;

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
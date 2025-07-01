using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static Checkpoint currentActiveCheckpoint; // Checkpoint được kích hoạt gần nhất

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.SetCheckpoint(transform.position);

        if (currentActiveCheckpoint != null && currentActiveCheckpoint != this)
        {
            currentActiveCheckpoint.ResetColor();
        }

        sr.color = Color.green;
        currentActiveCheckpoint = this;

        Debug.Log("Checkpoint triggered and saved!");
    }

    private void ResetColor()
    {
        if (sr != null)
            sr.color = Color.white; 
    }
}

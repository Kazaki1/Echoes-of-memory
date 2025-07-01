using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SendMessage("TakeDamage", damageAmount);
                player.transform.position = player.GetLastGroundedPosition();
            }
        }
    }
}

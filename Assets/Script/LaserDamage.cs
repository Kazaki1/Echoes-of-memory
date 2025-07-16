using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 0.5f; // Thời gian giữa 2 lần gây sát thương
    private float lastDamageTime;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageInterval)
            {
                SoulMover player = other.GetComponent<SoulMover>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}

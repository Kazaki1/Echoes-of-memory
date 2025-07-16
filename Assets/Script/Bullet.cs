using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed;

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > 20 || Mathf.Abs(transform.position.y) > 20)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoulMover player = other.GetComponent<SoulMover>();
            if (player != null)
            {
                player.TakeDamage(10); // Số damage có thể chỉnh tùy ý
            }
            Destroy(gameObject); // Hủy viên đạn sau khi trúng
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject); // tự hủy khi ra khỏi màn hình
    }
}

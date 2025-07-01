using UnityEngine;
using System;
using System.Collections.Generic;

public class BulletHellAttack : EnemyAttackBase
{
    public GameObject bulletPrefab;
    public float fireInterval = 0.05f;
    public float bulletSpeed = 5f;
    public float spiralSpeed = 10f;
    public int StopAttackTime = 3;

    private bool attacking = false;
    private float currentAngle = 4f;
    private int fireCount = 0;       // 🔥 Đếm số vòng đã bắn
    public int bulletCount = 8;
    public Action OnAttackFinished;

    private List<GameObject> activeBullets = new List<GameObject>(); // 🔥 Thêm dòng này

    public override void StartAttack()
    {
        attacking = true;
        currentAngle = 0f;

        InvokeRepeating(nameof(Fire), 0f, fireInterval);

        if (StopAttackTime > 0)
            Invoke(nameof(StopAttack), StopAttackTime);
    }

    public override void StopAttack()
    {
        if (!attacking) return;

        attacking = false;
        CancelInvoke(nameof(Fire));

        // 🔥 Tắt toàn bộ đạn còn lại
        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null)
                bullet.SetActive(false);
        }

        activeBullets.Clear(); // Dọn danh sách

        Debug.Log("Enemy attack stopped.");
        OnAttackFinished?.Invoke();
    }

    void Fire()
    {
        if (!attacking) return;

        float offset = (fireCount % 2 == 0) ? 0f : (360f / bulletCount) / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount) + offset + currentAngle;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet b = bullet.GetComponent<Bullet>();
            b.direction = dir.normalized;
            b.speed = bulletSpeed;

            activeBullets.Add(bullet);
        }

        fireCount++;
        currentAngle += spiralSpeed;
    }
}
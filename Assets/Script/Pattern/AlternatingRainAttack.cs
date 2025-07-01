using UnityEngine;
using System;
using System.Collections.Generic;

public class AlternatingRainAttack : EnemyAttackBase
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;

    [Header("Timing Settings")]
    public float fireInterval = 0.3f;
    public float delayBetweenBullets = 0.1f;
    public int StopAttackTime = 5;

    [Header("Spiral Settings")]
    public float spiralAngleStep = 30f; // Góc xoắn ốc mỗi lần bắn

    private bool attacking = false;
    private float currentAngle = 0f;

    public Action OnAttackFinished;
    private List<GameObject> activeBullets = new List<GameObject>();

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

        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null)
                bullet.SetActive(false);
        }

        activeBullets.Clear();
        Debug.Log("Alternating spiral attack stopped.");
        OnAttackFinished?.Invoke();
    }

    void Fire()
    {
        if (!attacking) return;

        int bulletCount = 12; // Số viên đạn trên vòng tròn
        float angleStep = 360f / bulletCount;
        float startAngle = currentAngle;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            SpawnBullet(angle);
        }

        currentAngle += spiralAngleStep; // Xoay vòng cho lần bắn tiếp theo
    }

    void SpawnBullet(float angle)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet b = bullet.GetComponent<Bullet>();
        b.direction = dir.normalized;
        b.speed = bulletSpeed;
        activeBullets.Add(bullet);
    }
}
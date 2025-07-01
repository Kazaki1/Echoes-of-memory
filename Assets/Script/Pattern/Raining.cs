using UnityEngine;
using System;
using System.Collections.Generic;

public class Raining : EnemyAttackBase
{
    public GameObject bulletPrefab;
    public float fireInterval = 0.2f;
    public float bulletSpeed = 5f;
    public float spawnRangeX = 6f;         // Phạm vi random theo chiều ngang
    public float spawnHeight = 0f;         // Chiều cao để spawn đạn
    public int StopAttackTime = 5;
    public int bulletsPerWave = 5;         // Bao nhiêu viên mỗi đợt bắn

    private bool attacking = false;
    public Action OnAttackFinished;

    private List<GameObject> activeBullets = new List<GameObject>();

    public override void StartAttack()
    {
        Debug.Log("Raining.StartAttack called");
        attacking = true;
        InvokeRepeating(nameof(Fire), 0f, fireInterval);

        if (StopAttackTime > 0)
            Invoke(nameof(StopAttack), StopAttackTime);
    }

    public void RegisterOnAttackFinished(Action callback)
    {
        OnAttackFinished += callback;
    }

    public override void StopAttack()
    {
        Debug.Log("Raining.StopAttack called");
        if (!attacking) return;

        attacking = false;
        CancelInvoke(nameof(Fire));

        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null)
                bullet.SetActive(false);
        }

        activeBullets.Clear();

        Debug.Log("Enemy attack stopped.");
        OnAttackFinished?.Invoke();
    }

    void Fire()
    {
        if (!attacking) return;

        for (int i = 0; i < bulletsPerWave; i++)
        {
            float randomX = UnityEngine.Random.Range(-spawnRangeX, spawnRangeX);
            Vector3 spawnPos = new Vector3(randomX, transform.position.y + spawnHeight, 0);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Bullet b = bullet.GetComponent<Bullet>();
            b.direction = Vector2.down; // Rơi xuống
            b.speed = bulletSpeed;

            activeBullets.Add(bullet);
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class AlternatingRainAttack : EnemyAttackBase
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;

    [Header("Timing Settings")]
    public float fireInterval = 0.8f;           // Khoảng cách giữa mỗi lần bắn (1 cặp)
    public float delayBetweenBullets = 0.15f;   // Delay giữa viên phải và viên trái
    public int stopAfterSeconds = 5;

    [Header("Offset Settings")]
    public float xOffset = 1.5f; // khoảng cách lệch trái–phải
    private bool alternate = false;

    private bool isAttacking = false;
    private List<GameObject> activeBullets = new List<GameObject>();

    public Action OnAttackFinished;

    public override void StartAttack()
    {
        if (isAttacking) return;

        Debug.Log("🔫 Rain Attack Started");
        isAttacking = true;
        alternate = false;

        InvokeRepeating(nameof(FirePair), 0f, fireInterval);
        Invoke(nameof(StopAttack), stopAfterSeconds);
    }

    public override void StopAttack()
    {
        if (!isAttacking) return;

        Debug.Log("🛑 Rain Attack Stopped");
        isAttacking = false;

        CancelInvoke(nameof(FirePair));

        foreach (var bullet in activeBullets)
        {
            if (bullet != null)
                Destroy(bullet);
        }
        activeBullets.Clear();

        OnAttackFinished?.Invoke();
    }

    void FirePair()
    {
        if (!isAttacking || bulletPrefab == null) return;

        float offsetAmount = alternate ? xOffset : -xOffset;
        alternate = !alternate;

        Vector2 rightPos = (Vector2)transform.position + new Vector2(offsetAmount, 0f);
        Vector2 leftPos = (Vector2)transform.position + new Vector2(-offsetAmount, 0f);
        Vector2 direction = Vector2.down;

        // Bắn viên bên phải ngay
        SpawnBullet(rightPos, direction);

        // Bắn viên bên trái sau delayBetweenBullets
        StartCoroutine(DelayedLeftBullet(leftPos, direction));
    }

    System.Collections.IEnumerator DelayedLeftBullet(Vector2 spawnPos, Vector2 dir)
    {
        yield return new WaitForSeconds(delayBetweenBullets);
        SpawnBullet(spawnPos, dir);
    }

    void SpawnBullet(Vector2 spawnPos, Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.direction = dir.normalized;
            b.speed = bulletSpeed;
            activeBullets.Add(bullet);
        }
        else
        {
            Debug.LogWarning("Bullet prefab is missing Bullet script!");
            Destroy(bullet);
        }
    }
}

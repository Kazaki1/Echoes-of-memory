using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AlternatingRainAttack : EnemyAttackBase
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;
    public int bulletsPerSide = 8;        // 🔥 Số viên mỗi bên
    public float spacing = 0.5f;          // 🔥 Khoảng cách giữa các viên đạn

    [Header("Timing Settings")]
    public float fireInterval = 0.8f;
    public float delayBetweenBullets = 0.15f;
    public int stopAfterSeconds = 5;

    [Header("Offset Settings")]
    public float xOffset = 1.5f;

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
                bullet.SetActive(false);
        }

        activeBullets.Clear();
        OnAttackFinished?.Invoke();
    }

    void FirePair()
    {
        if (!isAttacking || bulletPrefab == null) return;

        float offsetAmount = alternate ? xOffset : -xOffset;
        alternate = !alternate;

        Vector2 rightStart = (Vector2)transform.position + new Vector2(offsetAmount, 0f);
        Vector2 leftStart = (Vector2)transform.position + new Vector2(-offsetAmount, 0f);
        Vector2 direction = Vector2.down;

        // Bắn bên phải ngay
        FireRow(rightStart, direction);

        // Bắn bên trái sau delay
        StartCoroutine(DelayedLeftRow(leftStart, direction));
    }

    IEnumerator DelayedLeftRow(Vector2 leftStart, Vector2 dir)
    {
        yield return new WaitForSeconds(delayBetweenBullets);
        FireRow(leftStart, dir);
    }

    void FireRow(Vector2 center, Vector2 dir)
    {
        float totalWidth = (bulletsPerSide - 1) * spacing;

        for (int i = 0; i < bulletsPerSide; i++)
        {
            float offsetX = i * spacing - totalWidth / 2f;
            Vector2 spawnPos = new Vector2(center.x + offsetX, center.y);

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

    void OnDestroy()
    {
        Debug.Log("AlternatingRainAttack destroyed: " + gameObject.name);
    }

    void OnDisable()
    {
        Destroy(gameObject);
    }
}

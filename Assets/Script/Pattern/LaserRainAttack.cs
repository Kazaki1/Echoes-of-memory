using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LaserRainAttack : EnemyAttackBase
{
    [Header("Laser Settings")]
    public GameObject warningLaserPrefab;
    public GameObject laserPrefab;
    public float warningTime = 1f;
    public float laserDuration = 5f;
    public Transform[] firePositions;

    [Header("Rain Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public float fireInterval = 0.2f;
    public float spawnRangeX = 6f;
    public float spawnHeight = 0f;
    public int bulletsPerWave = 5;
    public Transform rainSpawnReference; // ✅ NEW: điểm gốc spawn mưa đạn

    public Action OnAttackFinished;

    private bool isAttacking = false;
    private GameObject activeLaser;
    private List<GameObject> activeBullets = new List<GameObject>();

    public override void StartAttack()
    {
        if (isAttacking) return;
        isAttacking = true;

        Debug.Log("LaserRainAttack.StartAttack()");
        StartCoroutine(AttackSequence());
    }

    public override void StopAttack()
    {
        if (!isAttacking) return;

        Debug.Log("LaserRainAttack.StopAttack()");
        isAttacking = false;

        CancelInvoke(nameof(FireRain));
        StopAllCoroutines();

        if (activeLaser != null)
            Destroy(activeLaser);

        foreach (GameObject bullet in activeBullets)
            if (bullet != null) bullet.SetActive(false);

        activeBullets.Clear();

        OnAttackFinished?.Invoke();
    }

    private IEnumerator AttackSequence()
    {
        // 1. Cảnh báo laser
        int index = UnityEngine.Random.Range(0, firePositions.Length);
        Vector3 laserPos = firePositions[index].position;

        GameObject warning = Instantiate(warningLaserPrefab, laserPos, Quaternion.identity, transform);
        yield return new WaitForSeconds(warningTime);
        Destroy(warning);

        // 2. Bắn laser
        activeLaser = Instantiate(laserPrefab, laserPos, Quaternion.identity, transform);

        // 3. Mưa đạn đồng thời
        InvokeRepeating(nameof(FireRain), 0f, fireInterval);

        // 4. Dừng sau laserDuration
        yield return new WaitForSeconds(laserDuration);

        StopAttack();
    }

    void FireRain()
    {
        if (!isAttacking || bulletPrefab == null || rainSpawnReference == null) return;

        Vector3 basePos = rainSpawnReference.position;

        for (int i = 0; i < bulletsPerWave; i++)
        {
            float randX = UnityEngine.Random.Range(-spawnRangeX, spawnRangeX);
            Vector3 spawnPos = new Vector3(basePos.x + randX, basePos.y + spawnHeight, basePos.z);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.direction = Vector2.down;
                b.speed = bulletSpeed;
                activeBullets.Add(bullet);
            }
            else
            {
                Debug.LogWarning("Bullet prefab thiếu script Bullet!");
                Destroy(bullet);
            }
        }
    }
}

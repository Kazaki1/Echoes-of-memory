using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAndBulletHellPattern2 : EnemyAttackBase
{
    [Header("Laser Setup")]
    public GameObject horizontalLaserPrefab;
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    public float laserSpeed = 5f;
    public float horizontalDistance = 10f;

    [Header("Bullet Hell Settings")]
    public GameObject bulletPrefab;
    public float fireInterval = 0.05f;
    public float bulletSpeed = 5f;
    public float spiralSpeed = 10f;
    public int bulletCount = 8;
    public float stopAttackTime = 3f;
    public Transform bulletHellSpawnPoint1;
    public Transform bulletHellSpawnPoint2;

    public Action OnAttackFinished;

    private List<GameObject> activeBullets = new List<GameObject>();
    private List<GameObject> activeLasers = new List<GameObject>();
    private float currentAngle = 0f;
    private int fireCount = 0;
    private bool attacking = false;

    public override void StartAttack()
    {
        if (attacking) return;
        attacking = true;
        StartCoroutine(AttackSequence());
    }

    public override void StopAttack()
    {
        if (!attacking) return;
        attacking = false;

        CancelInvoke(nameof(Fire));
        StopAllCoroutines();

        foreach (GameObject bullet in activeBullets)
            if (bullet != null)
                Destroy(bullet);
        activeBullets.Clear();

        foreach (GameObject laser in activeLasers)
            if (laser != null)
                Destroy(laser);
        activeLasers.Clear();

        OnAttackFinished?.Invoke();
    }

    private IEnumerator AttackSequence()
    {
        // 1. Spawn laser
        GameObject leftLaser = Instantiate(horizontalLaserPrefab, leftSpawnPoint.position, Quaternion.identity);
        GameObject rightLaser = Instantiate(horizontalLaserPrefab, rightSpawnPoint.position, Quaternion.identity);
        activeLasers.Add(leftLaser);
        activeLasers.Add(rightLaser);

        // 2. Di chuyển laser
        Coroutine moveLeft = StartCoroutine(MoveLaser(leftLaser.transform, Vector2.right, horizontalDistance));
        Coroutine moveRight = StartCoroutine(MoveLaser(rightLaser.transform, Vector2.left, horizontalDistance));
        yield return moveLeft;
        yield return moveRight;

        // 3. Bắt đầu bullet hell từ 2 điểm
        currentAngle = 0f;
        fireCount = 0;
        InvokeRepeating(nameof(Fire), 0f, fireInterval);

        if (stopAttackTime > 0)
            Invoke(nameof(StopAttack), stopAttackTime);
    }

    private IEnumerator MoveLaser(Transform laser, Vector2 dir, float distance)
    {
        float traveled = 0f;
        while (traveled < distance)
        {
            float step = laserSpeed * Time.deltaTime;
            laser.Translate(dir * step);
            traveled += step;
            yield return null;
        }
    }

    private void Fire()
    {
        if (!attacking) return;

        float offset = (fireCount % 2 == 0) ? 0f : (360f / bulletCount) / 2f;
        Vector3 spawnPos1 = bulletHellSpawnPoint1 != null ? bulletHellSpawnPoint1.position : transform.position;
        Vector3 spawnPos2 = bulletHellSpawnPoint2 != null ? bulletHellSpawnPoint2.position : transform.position;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount) + offset + currentAngle;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Bắn từ vị trí 1
            GameObject bullet1 = Instantiate(bulletPrefab, spawnPos1, Quaternion.identity);
            Bullet b1 = bullet1.GetComponent<Bullet>();
            b1.direction = dir.normalized;
            b1.speed = bulletSpeed;
            activeBullets.Add(bullet1);

            // Bắn từ vị trí 2
            GameObject bullet2 = Instantiate(bulletPrefab, spawnPos2, Quaternion.identity);
            Bullet b2 = bullet2.GetComponent<Bullet>();
            b2.direction = dir.normalized;
            b2.speed = bulletSpeed;
            activeBullets.Add(bullet2);
        }

        fireCount++;
        currentAngle += spiralSpeed;
    }
}

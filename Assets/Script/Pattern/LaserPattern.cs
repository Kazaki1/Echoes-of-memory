using System;
using System.Collections;
using UnityEngine;

public class LaserPattern : EnemyAttackBase
{
    [Header("Prefabs")]
    public GameObject warningLaserPrefab;
    public GameObject laserPrefab;

    [Header("Timing Settings")]
    public float warningTime = 1f;
    public float laserDuration = 1.5f;
    public float attackInterval = 2f;
    public int maxAttacks = 3;

    [Header("Laser Settings")]
    public Transform fireOrigin;
    public int laserCount = 6;
    public float angleStep = 60f;
    public float angleOffsetPerAttack = 0f;

    [Tooltip("Tùy chọn góc bắt đầu cho từng lượt bắn. Nếu rỗng hoặc không đủ phần tử, sẽ dùng công thức mặc định.")]
    public float[] customStartAngles;

    public Action OnAttackFinished;

    private bool attacking = false;

    public override void StartAttack()
    {
        if (attacking) return;

        attacking = true;
        StartCoroutine(AttackLoop());
    }

    public override void StopAttack()
    {
        if (!attacking) return;

        attacking = false;
        StopAllCoroutines();

        OnAttackFinished?.Invoke();
    }

    private IEnumerator AttackLoop()
    {
        int attackCount = 0;

        while (attacking && attackCount < maxAttacks)
        {
            // ✅ Lấy góc bắt đầu:
            float startingAngle;
            if (customStartAngles != null && attackCount < customStartAngles.Length)
                startingAngle = customStartAngles[attackCount];
            else
                startingAngle = attackCount * angleOffsetPerAttack;

            GameObject[] warnings = new GameObject[laserCount];

            // Spawn cảnh báo
            for (int i = 0; i < laserCount; i++)
            {
                float angle = startingAngle + i * angleStep;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                warnings[i] = Instantiate(warningLaserPrefab, fireOrigin.position, rotation, transform);
            }

            yield return new WaitForSeconds(warningTime);

            foreach (var warn in warnings)
                Destroy(warn);

            GameObject[] lasers = new GameObject[laserCount];
            for (int i = 0; i < laserCount; i++)
            {
                float angle = startingAngle + i * angleStep;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                lasers[i] = Instantiate(laserPrefab, fireOrigin.position, rotation, transform);
            }

            yield return new WaitForSeconds(laserDuration);

            foreach (var laser in lasers)
                Destroy(laser);

            attackCount++;
            yield return new WaitForSeconds(attackInterval);
        }

        StopAttack();
    }
}

using System;
using System.Collections;
using UnityEngine;

public class DiagonalThenRotateLaser : EnemyAttackBase
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
    public int laserCount = 4; // chỉ 4 để bắn chéo: 45, 135, 225, 315
    public float[] diagonalAngles = new float[] { 45f, 135f, 225f, 315f }; // bắn chéo
    public float angleOffsetPerAttack = 0f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // độ/giây

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
            GameObject[] warnings = new GameObject[laserCount];

            // Giai đoạn 1: Cảnh báo bắn chéo
            for (int i = 0; i < laserCount; i++)
            {
                float angle = diagonalAngles[i] + attackCount * angleOffsetPerAttack;
                Quaternion rot = Quaternion.Euler(0, 0, angle);
                warnings[i] = Instantiate(warningLaserPrefab, fireOrigin.position, rot, transform);
            }

            yield return new WaitForSeconds(warningTime);

            foreach (var warn in warnings)
                Destroy(warn);

            // Giai đoạn 2: Tạo laser chéo, sau đó quay
            GameObject[] lasers = new GameObject[laserCount];
            Transform[] laserTransforms = new Transform[laserCount];

            for (int i = 0; i < laserCount; i++)
            {
                float angle = diagonalAngles[i] + attackCount * angleOffsetPerAttack;
                Quaternion rot = Quaternion.Euler(0, 0, angle);
                GameObject laser = Instantiate(laserPrefab, fireOrigin.position, rot, transform);
                lasers[i] = laser;
                laserTransforms[i] = laser.transform;
            }

            float elapsed = 0f;
            while (elapsed < laserDuration)
            {
                float rotateAmount = rotationSpeed * Time.deltaTime;
                foreach (Transform laser in laserTransforms)
                {
                    laser.RotateAround(fireOrigin.position, Vector3.forward, -rotateAmount); // quay chiều kim
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (var laser in lasers)
                Destroy(laser);

            attackCount++;
            yield return new WaitForSeconds(attackInterval);
        }

        StopAttack();
    }
}

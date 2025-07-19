using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LaserAttackConfig
{
    public int[] fireIndices; // Các chỉ số vị trí sẽ bắn laser trong đợt này
}

public class LaserAttackPattern1 : EnemyAttackBase
{
    [Header("Prefabs")]
    public GameObject warningLaserPrefab;
    public GameObject laserPrefab;

    [Header("Timing Settings")]
    public float warningTime = 1f;
    public float laserDuration = 1.5f;
    public float attackInterval = 2f;

    [Header("Laser Settings")]
    public List<LaserAttackConfig> customAttackPatterns = new(); // ✅ Danh sách pattern theo từng đợt

    [Header("Laser Spawn Positions")]
    public Transform[] firePositions;

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
        for (int i = 0; i < customAttackPatterns.Count && attacking; i++)
        {
            LaserAttackConfig currentPattern = customAttackPatterns[i];
            List<GameObject> warnings = new();
            List<GameObject> lasers = new();

            // ✅ Hiện cảnh báo tại các vị trí chỉ định
            foreach (int index in currentPattern.fireIndices)
            {
                if (index < 0 || index >= firePositions.Length) continue;

                Vector3 pos = firePositions[index].position;
                GameObject warning = Instantiate(warningLaserPrefab, pos, Quaternion.identity, transform);
                warnings.Add(warning);
            }

            yield return new WaitForSeconds(warningTime);

            // Xoá cảnh báo
            foreach (GameObject warning in warnings)
                Destroy(warning);

            // Bắn laser
            foreach (int index in currentPattern.fireIndices)
            {
                if (index < 0 || index >= firePositions.Length) continue;

                Vector3 pos = firePositions[index].position;
                GameObject laser = Instantiate(laserPrefab, pos, Quaternion.identity, transform);
                lasers.Add(laser);
            }

            yield return new WaitForSeconds(laserDuration);

            foreach (GameObject laser in lasers)
                Destroy(laser);

            yield return new WaitForSeconds(attackInterval);
        }

        StopAttack();
    }
}

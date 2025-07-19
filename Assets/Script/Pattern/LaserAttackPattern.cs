using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttackPattern : EnemyAttackBase
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
    public int lasersPerAttack = 2; // ✅ Số lượng laser mỗi lần tấn công

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
        int attackCount = 0;

        while (attacking && attackCount < maxAttacks)
        {
            // ✅ Chọn vị trí ngẫu nhiên không trùng lặp
            List<int> selectedIndices = GetUniqueRandomIndices(lasersPerAttack, firePositions.Length);
            List<GameObject> warnings = new List<GameObject>();
            List<GameObject> lasers = new List<GameObject>();

            // Hiện cảnh báo
            foreach (int index in selectedIndices)
            {
                Vector3 pos = firePositions[index].position;
                GameObject warning = Instantiate(warningLaserPrefab, pos, Quaternion.identity, transform);
                warnings.Add(warning);
            }

            yield return new WaitForSeconds(warningTime);

            // Xóa cảnh báo
            foreach (GameObject warning in warnings)
                Destroy(warning);

            // Bắn laser
            foreach (int index in selectedIndices)
            {
                Vector3 pos = firePositions[index].position;
                GameObject laser = Instantiate(laserPrefab, pos, Quaternion.identity, transform);
                lasers.Add(laser);
            }

            yield return new WaitForSeconds(laserDuration);

            foreach (GameObject laser in lasers)
                Destroy(laser);

            attackCount++;
            yield return new WaitForSeconds(attackInterval);
        }

        StopAttack();
    }

    // ✅ Hàm lấy index ngẫu nhiên không trùng
    private List<int> GetUniqueRandomIndices(int count, int max)
    {
        List<int> indices = new List<int>();
        List<int> available = new List<int>();

        for (int i = 0; i < max; i++) available.Add(i);

        for (int i = 0; i < Mathf.Min(count, max); i++)
        {
            int randIndex = UnityEngine.Random.Range(0, available.Count);
            indices.Add(available[randIndex]);
            available.RemoveAt(randIndex);
        }

        return indices;
    }
}

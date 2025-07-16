using System;
using System.Collections;
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

    [Header("Laser Spawn Positions")]
    public Transform[] firePositions; // Ba vị trí Top, Mid, Bottom

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

        OnAttackFinished?.Invoke(); // Báo BattleManager kết thúc lượt
    }

    private IEnumerator AttackLoop()
    {
        int attackCount = 0;

        while (attacking && attackCount < maxAttacks)
        {
            // Random 2 vị trí khác nhau
            int index1 = UnityEngine.Random.Range(0, firePositions.Length);
            int index2;
            do
            {
                index2 = UnityEngine.Random.Range(0, firePositions.Length);
            } while (index2 == index1);

            Vector3 pos1 = firePositions[index1].position;
            Vector3 pos2 = firePositions[index2].position;

            // Cảnh báo laser
            GameObject warning1 = Instantiate(warningLaserPrefab, pos1, Quaternion.identity, transform);
            GameObject warning2 = Instantiate(warningLaserPrefab, pos2, Quaternion.identity, transform);
            yield return new WaitForSeconds(warningTime);

            Destroy(warning1);
            Destroy(warning2);

            // Bắn laser
            GameObject laser1 = Instantiate(laserPrefab, pos1, Quaternion.identity, transform);
            GameObject laser2 = Instantiate(laserPrefab, pos2, Quaternion.identity, transform);
            yield return new WaitForSeconds(laserDuration);

            Destroy(laser1);
            Destroy(laser2);

            attackCount++;
            yield return new WaitForSeconds(attackInterval);
        }

        StopAttack(); // ✅ Gọi StopAttack để đảm bảo dọn dẹp và callback
    }
}

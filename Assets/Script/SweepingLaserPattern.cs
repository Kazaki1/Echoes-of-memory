using System;
using System.Collections;
using UnityEngine;

public class SweepingLaserPattern : EnemyAttackBase
{
    [Header("Laser Prefabs")]
    public GameObject horizontalLaserPrefab; // ↔
    public GameObject verticalLaserPrefab;   // ↕ 

    [Header("Laser Movement Settings")]
    public float laserSpeed = 5f;
    public float horizontalDistance = 10f;
    public float verticalDistance = 6f;

    [Header("Attack Settings")]
    public float attackInterval = 2f;
    public int maxAttacks = 3;

    [Header("Laser Spawn Points")]
    public Transform leftSpawn;
    public Transform rightSpawn;
    public Transform topSpawn;
    public Transform bottomSpawn;

    public Action OnAttackFinished;

    private bool attacking = false;

    public override void StartAttack()
    {
        if (attacking) return;
        attacking = true;
        StartCoroutine(SweepAttackLoop());
    }

    public override void StopAttack()
    {
        if (!attacking) return;
        attacking = false;
        StopAllCoroutines();
        OnAttackFinished?.Invoke();
    }

    private IEnumerator SweepAttackLoop()
    {
        int attackCount = 0;

        while (attacking && attackCount < maxAttacks)
        {
            int direction = UnityEngine.Random.Range(0, 4); // 0-3
            Transform spawn = null;
            Vector2 moveDir = Vector2.zero;
            float distanceToTravel = 0f;
            GameObject selectedLaserPrefab = null;

            switch (direction)
            {
                case 0: // Trái → Phải
                    spawn = leftSpawn;
                    moveDir = Vector2.right;
                    distanceToTravel = horizontalDistance;
                    selectedLaserPrefab = horizontalLaserPrefab;
                    break;
                case 1: // Phải → Trái
                    spawn = rightSpawn;
                    moveDir = Vector2.left;
                    distanceToTravel = horizontalDistance;
                    selectedLaserPrefab = horizontalLaserPrefab;
                    break;
                case 2: // Trên ↓ Dưới
                    spawn = topSpawn;
                    moveDir = Vector2.down;
                    distanceToTravel = verticalDistance;
                    selectedLaserPrefab = verticalLaserPrefab;
                    break;
                case 3: // Dưới ↑ Trên
                    spawn = bottomSpawn;
                    moveDir = Vector2.up;
                    distanceToTravel = verticalDistance;
                    selectedLaserPrefab = verticalLaserPrefab;
                    break;
            }

            if (spawn != null && selectedLaserPrefab != null)
            {
                GameObject laser = Instantiate(selectedLaserPrefab, spawn.position, Quaternion.identity);
                StartCoroutine(MoveLaser(laser.transform, moveDir, distanceToTravel));
            }

            float estimatedDuration = distanceToTravel / laserSpeed;
            yield return new WaitForSeconds(estimatedDuration + attackInterval);

            attackCount++;
        }

        StopAttack();
    }

    private IEnumerator MoveLaser(Transform laser, Vector2 dir, float distance)
    {
        float traveled = 0f;

        while (traveled < distance)
        {
            float moveStep = laserSpeed * Time.deltaTime;
            laser.Translate(dir * moveStep);
            traveled += moveStep;
            yield return null;
        }

        Destroy(laser.gameObject);
    }
}

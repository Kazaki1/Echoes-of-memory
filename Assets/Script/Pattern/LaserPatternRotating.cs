using System;
using System.Collections;
using UnityEngine;

public class LaserPatternWithDiagonalStart : EnemyAttackBase
{
    [Header("Prefabs")]
    public GameObject diagonalLaserPrefab;
    public GameObject laserHorizontalPrefab;
    public GameObject laserVerticalPrefab;

    [Header("Timing Settings")]
    public float warningTime = 1f;
    public float laserDuration = 1.5f;
    public float attackInterval = 2f;
    public int maxAttacks = 3;

    [Header("Grid Settings")]
    public Transform fireOrigin;
    public int horizontalCount = 3;
    public int verticalCount = 3;
    public float spacing = 1f;
    public float angleOffsetPerAttack = 45f;

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
        Vector3 center = fireOrigin != null ? fireOrigin.position : transform.position;

        while (attacking && attackCount < maxAttacks)
        {
            GameObject[] warnings;
            GameObject[] lasers;

            if (attackCount == 0)
            {
                // 🔰 BẮN CHÉO 4 HƯỚNG
                float[] diagonalAngles = { 45f, 135f, 225f, 315f };
                warnings = new GameObject[diagonalAngles.Length];

                for (int i = 0; i < diagonalAngles.Length; i++)
                {
                    Quaternion rot = Quaternion.Euler(0f, 0f, diagonalAngles[i]);
                    warnings[i] = Instantiate(diagonalLaserPrefab, center, rot, transform);
                }

                yield return new WaitForSeconds(warningTime);
                foreach (var warn in warnings) if (warn) Destroy(warn);

                lasers = new GameObject[diagonalAngles.Length];
                for (int i = 0; i < diagonalAngles.Length; i++)
                {
                    Quaternion rot = Quaternion.Euler(0f, 0f, diagonalAngles[i]);
                    lasers[i] = Instantiate(diagonalLaserPrefab, center, rot, transform);
                }

                yield return new WaitForSeconds(laserDuration);
                foreach (var laser in lasers) if (laser) Destroy(laser);
            }
            else
            {
                // 🔁 SPAWN LƯỚI XOAY
                float currentRotation = (attackCount - 1) * angleOffsetPerAttack;
                Quaternion rotation = Quaternion.AngleAxis(currentRotation, Vector3.forward);

                Vector3[] horizontalPositions = GenerateGrid(center, horizontalCount, spacing, Axis.Y, rotation);
                Vector3[] verticalPositions = GenerateGrid(center, verticalCount, spacing, Axis.X, rotation);

                warnings = new GameObject[horizontalPositions.Length + verticalPositions.Length];
                int index = 0;

                foreach (var pos in horizontalPositions)
                    warnings[index++] = Instantiate(laserHorizontalPrefab, pos, rotation, transform);
                foreach (var pos in verticalPositions)
                    warnings[index++] = Instantiate(laserVerticalPrefab, pos, rotation, transform);

                yield return new WaitForSeconds(warningTime);
                foreach (var warn in warnings) if (warn) Destroy(warn);

                lasers = new GameObject[horizontalPositions.Length + verticalPositions.Length];
                index = 0;

                foreach (var pos in horizontalPositions)
                    lasers[index++] = Instantiate(laserHorizontalPrefab, pos, rotation, transform);
                foreach (var pos in verticalPositions)
                    lasers[index++] = Instantiate(laserVerticalPrefab, pos, rotation, transform);

                yield return new WaitForSeconds(laserDuration);
                foreach (var laser in lasers) if (laser) Destroy(laser);
            }

            attackCount++;
            yield return new WaitForSeconds(attackInterval);
        }

        StopAttack();
    }

    enum Axis { X, Y }

    private Vector3[] GenerateGrid(Vector3 center, int count, float spacing, Axis axis, Quaternion rotation)
    {
        Vector3[] positions = new Vector3[count];
        int half = count / 2;

        for (int i = 0; i < count; i++)
        {
            int offset = i - half;
            if (count % 2 == 0 && offset >= 0) offset++;

            Vector3 pos = axis == Axis.X
                ? new Vector3(center.x + offset * spacing, center.y, center.z)
                : new Vector3(center.x, center.y + offset * spacing, center.z);

            positions[i] = RotateAroundPoint(pos, center, rotation);
        }

        return positions;
    }

    private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }
}

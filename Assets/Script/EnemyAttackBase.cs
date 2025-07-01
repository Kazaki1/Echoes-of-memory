using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    public abstract void StartAttack();
    public virtual void StopAttack() { } 
}

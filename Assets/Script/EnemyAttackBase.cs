using UnityEngine;
using System;

public abstract class EnemyAttackBase : MonoBehaviour
{
    public Action OnAttackFinished; 

    public abstract void StartAttack();
    public virtual void StopAttack() { }

}

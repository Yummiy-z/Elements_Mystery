using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData",menuName = "Attack/EnemyAttackData")]
public class AttackData_SO : ScriptableObject
{
   public int attackRange;
   public int attackDamage;
   
   public int skillAttackRange;
   public int skillAttackDamage;
   public float coolDown;
}

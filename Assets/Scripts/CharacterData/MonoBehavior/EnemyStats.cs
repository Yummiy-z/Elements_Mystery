using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyBaseData_SO templateData;

    public AttackData_SO enemyAttackData;
    public EnemyBaseData_SO enemyBaseData;

    private void Awake()
    {
        if (templateData != null)
        {
            enemyBaseData = Instantiate(templateData);
        }
    }

    public ShieldType ShieldProperty
    {
        get { return enemyBaseData != null ? enemyBaseData.shieldType : ShieldType.Wood; }
        set { enemyBaseData.shieldType = value; }
    }

    public EnemyType EnemyProperty
    {
        get { return enemyBaseData != null ? enemyBaseData.enemyType : EnemyType.Wood; }
        set { enemyBaseData.enemyType = value; }
    }

    public int MaxShieldValue
    {
        get { return enemyBaseData != null ? enemyBaseData.maxShieldValue : 0; }
        set { enemyBaseData.maxShieldValue = value; }
    }

    public int CurrentShieldValue
    {
        get { return enemyBaseData != null ? enemyBaseData.currentShieldValue : 0; }
        set { enemyBaseData.currentShieldValue = value; }
    }

    public int MaxHealth
    {
        get { return enemyBaseData != null ? enemyBaseData.maxHealth : 0; }
        set { enemyBaseData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { return enemyBaseData != null ? enemyBaseData.currentHealth : 0; }
        set { enemyBaseData.currentHealth = value; }
    }

    public int AttackRange
    {
        get { return enemyAttackData != null ? enemyAttackData.attackRange : 0; }
        set { enemyAttackData.attackRange = value; }
    }

    public int AttackDamage
    {
        get { return enemyAttackData != null ? enemyAttackData.attackDamage : 0; }
        set { enemyAttackData.attackDamage = value; }
    }

    public int SkillAttackRange
    {
        get { return enemyAttackData != null ? enemyAttackData.skillAttackRange : 0; }
        set { enemyAttackData.skillAttackRange = value; }
    }

    public int SkillAttackDamage
    {
        get { return enemyAttackData != null ? enemyAttackData.skillAttackDamage : 0; }
        set { enemyAttackData.skillAttackDamage = value; }
    }
}
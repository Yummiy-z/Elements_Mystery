using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldType
{
    Water,Wood,Flame
}
 
public enum EnemyType
{
    Gold,
    Wood,
    Water,
    Flame,
    Soil,
}
[CreateAssetMenu(fileName = "EnemyData",menuName = "CharacterBase/EnemyData")]
public class EnemyBaseData_SO : ScriptableObject
{
    public int maxShieldValue;
    public int currentShieldValue;
    public ShieldType shieldType;
    public EnemyType enemyType;
    public int maxHealth;
    public int currentHealth;
    


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData",menuName = "CharacterBase/PlayerData")]
public class PlayerBaseData_SO : ScriptableObject
{
    //基础数据中的重力gravityDownForce
    //需要用于游戏运行的计算的基础数据才放到这个模板
    [Header("Stats Info")] public int maxHealth;
    public int currentHealth;
    public int maxShieldValue;
    public int currentShieldValue;
    public int maxStandardBulletsNumber;
    public int currentStandardBulletsNumber;
    public int maxLargeBulletsNumber;
    public int currentLargeBulletsNumber;
    public int maxSpecialBulletsNumber;
    public int currentSpecialBulletsNumber;
    public float walkSpeed;
    public float jumpHeight;
    public float sprintDistance;
    public int maxSprintNumbers;
    public float sprintCoolDown;
}

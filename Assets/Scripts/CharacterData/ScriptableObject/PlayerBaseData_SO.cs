using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData",menuName = "Player/Data")]
public class PlayerBaseData_SO : ScriptableObject
{
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


}

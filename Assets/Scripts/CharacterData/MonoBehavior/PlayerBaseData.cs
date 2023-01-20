using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseData : MonoBehaviour
{
    //数据模板，这样每次进入游戏就不用更改
    public PlayerBaseData_SO templateData;

    //原始数据
    public PlayerBaseData_SO playerData;

    private void Awake()
    {
        if (templateData != null)
        {
            playerData = Instantiate(templateData);
        }
    }

    #region Read From PlayerBaseData

    public int MaxHealth
    {
        get { return playerData != null ? playerData.maxHealth : 0; }
        set { playerData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { return playerData != null ? playerData.currentHealth : 0; }
        set { playerData.currentHealth = value; }
    }

    public int MaxShieldValue
    {
        get { return playerData != null ? playerData.maxShieldValue : 0; }
        set { playerData.maxShieldValue = value; }
    }

    public int CurrentShieldValue
    {
        get { return playerData != null ? playerData.currentShieldValue : 0; }
        set { playerData.currentShieldValue = value; }
    }

    public int MaxStandardBulletsNumber
    {
        get { return playerData != null ? playerData.maxStandardBulletsNumber : 0; }
        set { playerData.maxStandardBulletsNumber = value; }
    }

    public int CurrentStandardBulletsNumber
    {
        get { return playerData != null ? playerData.currentStandardBulletsNumber : 0; }
        set { playerData.currentStandardBulletsNumber = value; }
    }

    public int MaxLargeBulletsNumber
    {
        get { return playerData != null ? playerData.maxLargeBulletsNumber : 0; }
        set { playerData.maxLargeBulletsNumber = value; }
    }

    public int CurrentLargeBulletsNumber
    {
        get { return playerData != null ? playerData.currentLargeBulletsNumber : 0; }
        set { playerData.currentLargeBulletsNumber = value; }
    }

    public int MaxSpecialBulletsNumber
    {
        get { return playerData != null ? playerData.maxSpecialBulletsNumber : 0; }
        set { playerData.maxSpecialBulletsNumber = value; }
    }

    public int CurrentSpecialBulletsNumber
    {
        get { return playerData != null ? playerData.currentSpecialBulletsNumber : 0; }
        set { playerData.currentSpecialBulletsNumber = value; }
    }

    #endregion
}
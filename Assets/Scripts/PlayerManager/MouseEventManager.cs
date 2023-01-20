using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEventManager : Singleton<MouseEventManager>
{
    /// <summary>
    /// 鼠标掌握的射击事件
    /// </summary>
    public UnityAction OnMouseSingleShoot;
    public UnityAction OnMouseTripleShoot;
    public UnityAction OnMouseFullyAutoShoot;
    /// <summary>
    /// 鼠标的武器技能事件
    /// </summary>
    public UnityAction OnMouseWeaponSpecialShoot;

    public MouseEventManager(UnityAction onMouseSingleShoot)
    {
        OnMouseSingleShoot = onMouseSingleShoot;
    }

    private void Update()
    {
        WeaponShoot();
    }

    private void WeaponShoot()
    {
        if (PlayerInputHandler.Instance.GetSingleFireInput())
        {
            
        }
    }
}

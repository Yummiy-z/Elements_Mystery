using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponManager : MonoBehaviour
{
    //初始武器列表
    public List<WeaponController> startingWeapons = new List<WeaponController>();

    //武器位置
    public Transform weaponParentSocket;

    public Camera weaponCamera;

    public UnityAction<WeaponController> OnSwitchToWeapon;

    //玩家武器包，一个有9个格子(武器插槽)
    private WeaponController[] _weaponSlots = new WeaponController[3];

    private WeaponController activeWeapon;
    private PlayerInputHandler _inputHandler;

    private void Start()
    {
        //Unity 事件 OnWeaponSwitched方法订阅OnSwitchToWeapon事件
        _inputHandler = GetComponentInParent<PlayerInputHandler>();
        OnSwitchToWeapon += OnWeaponSwitched;
        foreach (WeaponController weapon in startingWeapons)
        {
            AddWeapon(weapon);
        }

        //显示武器格的枪(开关武器)
        SwitchWeapon();
        activeWeapon = _weaponSlots[0];
    }

    private void Update()
    {
        if (activeWeapon)
        {
            switch (activeWeapon.fireType)
            {
                case WeaponController.FireType.FullyAutoFire:
                    activeWeapon.HandleShootInput(_inputHandler.GetKeepFiringInput());
                    break;
                case WeaponController.FireType.SingleFire:
                    activeWeapon.HandleShootInput(_inputHandler.GetSingleFireInput());
                    break;
                case WeaponController.FireType.TripleFire:
                    activeWeapon.HandleShootInput(_inputHandler.GetSingleFireInput());
                    break;
                
            }
        }
    }

    private bool AddWeapon(WeaponController weaponPrefab)
    {
        //将武器背包格充满
        for (int i = 0; i < _weaponSlots.Length; i++)
        {
            //首先要检查格子是否为空
            if (_weaponSlots[i] == null)
            {
                WeaponController weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;

                weaponInstance.Owner = gameObject;
                weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                weaponInstance.ShowWeapon(false);

                _weaponSlots[i] = weaponInstance;
                return true;
            }
        }

        return false;
    }

    //显示武器格的枪(开关武器)
    public void SwitchWeapon()
    {
        SwitchWeaponToIndex(0);
    }

    //根据序号发布事件
    public void SwitchWeaponToIndex(int newWeaponIndex)
    {
        if (newWeaponIndex >= 0)
        {
            WeaponController newWeapon = GetWeaponAtSlotIndex(newWeaponIndex);
            //Invoke发布事件，该事件有订阅者即执行Invoke
            OnSwitchToWeapon?.Invoke(newWeapon);
        }
    }

    public WeaponController GetWeaponAtSlotIndex(int index)
    {
        if (index >= 0 && index < _weaponSlots.Length)
        {
            return _weaponSlots[index];
        }

        return null;
    }

    //显示武器
    private void OnWeaponSwitched(WeaponController newWeapon)
    {
        if (newWeapon != null)
        {
            newWeapon.ShowWeapon(true);
        }
    }
}
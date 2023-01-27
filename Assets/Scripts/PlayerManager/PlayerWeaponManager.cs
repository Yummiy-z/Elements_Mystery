using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerWeaponManager : Singleton<PlayerWeaponManager>
{
    //初始武器列表，使用列表给扩展空间
    public List<WeaponBase> startingWeapons = new List<WeaponBase>();

    //武器位置
    public Transform weaponParentSocket;

    public Camera weaponCamera;


    //玩家武器包，一个有4个格子(武器插槽)，主武器，副武器，初始武器手枪，替换武器临时格
    private WeaponBase[] _weaponSlots = new WeaponBase[4];

    //当前的手持武器，只有一个
    private WeaponBase _activeWeapon;
    private const float SwitchWeaponCoolDown = 0.5f;
    private float _lastSwitchWeaponTime;

    private PlayerInputHandler _inputHandler;

    //武器切换的动画
    //private Animator _animator;
    private bool _isSwitchWeapon;

    // protected  override void Awake()
    // {
    //     base.Awake();
    //     //_animator = GameObject.Find("WeaponParentPosition").GetComponent<Animator>();
    // }
    private void Start()
    {
        //首先将我的默认武器列表中的武器放入武器格
        foreach (WeaponBase weapon in startingWeapons)
        {
            StartAddWeapon(weapon);
        }

        _activeWeapon = _weaponSlots[2];
        _activeWeapon.isWeaponActive = true;
        _activeWeapon.ShowWeapon();
        _lastSwitchWeaponTime = 0f;
    }

    private void Update()
    {
        //武器切换流程
        //检测是否可以切换武器-是否要切换武器-切换指令发出-执行当前武器收起动画-在动画结束后执行消失-把新的武器显示出来-执行新的武器拿出动画
        _lastSwitchWeaponTime -= Time.deltaTime;
        SwitchWeaponCheck();
        //武器是否开枪
        WeaponShootCheck();
    }

    private void SwitchWeaponCheck()
    {
        if (PlayerInputHandler.Instance.SwitchWeaponInput() && _lastSwitchWeaponTime <= 0)
        {
            if (_activeWeapon != _weaponSlots[PlayerInputHandler.Instance.SwitchWeaponNumberInput() - 1] &&
                _weaponSlots[PlayerInputHandler.Instance.SwitchWeaponNumberInput() - 1] != null)
            {
                _isSwitchWeapon = true;
                //todo:播放武器收起动画
                _activeWeapon.GetComponentInChildren<Animator>().SetTrigger("SwitchDown");
                _activeWeapon.isWeaponActive = false;
                _activeWeapon.HideWeapon();
                _activeWeapon = _weaponSlots[PlayerInputHandler.Instance.SwitchWeaponNumberInput() - 1];
                //todo：播放武器拿起动画
                _activeWeapon.isWeaponActive = true;
                _activeWeapon.ShowWeapon();
                _activeWeapon.GetComponentInChildren<Animator>().SetTrigger("SwitchUp");
                _isSwitchWeapon = false;
                _lastSwitchWeaponTime = SwitchWeaponCoolDown;
            }
        }
    }

    private void WeaponShootCheck()
    {
        if (_activeWeapon.isWeaponActive && _isSwitchWeapon == false) 
        {
            switch (_activeWeapon.fireType)
            {
                case WeaponBase.FireType.SingleFire:
                    _activeWeapon.HandleShootInput(PlayerInputHandler.Instance.GetSingleFireInput());
                    break;
                case WeaponBase.FireType.TripleFire:
                    _activeWeapon.HandleShootInput(PlayerInputHandler.Instance.GetSingleFireInput());
                    break;
                case WeaponBase.FireType.FullyAutoFire:
                    _activeWeapon.HandleShootInput(PlayerInputHandler.Instance.GetKeepFiringInput());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    private void StartAddWeapon(WeaponBase weaponPrefab)
    {
        if (_weaponSlots[2] == null)
        {
            WeaponBase weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            var transform1 = weaponInstance.transform;
            transform1.localPosition = Vector3.zero;
            transform1.localRotation = Quaternion.identity;
            weaponInstance.Owner = gameObject;
            weaponInstance.SourcePrefab = weaponPrefab.gameObject;
            _weaponSlots[2] = weaponInstance;
            _weaponSlots[2].isWeaponActive = false;
            _weaponSlots[2].HideWeapon();
            //没有显示出来前，武器的位置不是射击位置
            return;
        }

        if (_weaponSlots[0] == null)
        {
            WeaponBase weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            var transform1 = weaponInstance.transform;
            transform1.localPosition = Vector3.zero;
            transform1.localRotation = Quaternion.identity;
            weaponInstance.Owner = gameObject;
            weaponInstance.SourcePrefab = weaponPrefab.gameObject;
            _weaponSlots[0] = weaponInstance;
            _weaponSlots[0].isWeaponActive = false;
            _weaponSlots[0].HideWeapon();
            return;
        }

        if (_weaponSlots[1] == null)
        {
            WeaponBase weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            var transform1 = weaponInstance.transform;
            transform1.localPosition = Vector3.zero;
            transform1.localRotation = Quaternion.identity;
            weaponInstance.Owner = gameObject;
            weaponInstance.SourcePrefab = weaponPrefab.gameObject;
            _weaponSlots[1] = weaponInstance;
            _weaponSlots[1].isWeaponActive = false;
            _weaponSlots[1].HideWeapon();
        }
    }
}
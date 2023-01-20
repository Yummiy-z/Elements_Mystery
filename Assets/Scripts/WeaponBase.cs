using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting.Dependencies.NCalc;


public class WeaponBase : MonoBehaviour
{
    //武器类型
    [EnumToggleButtons]
    public enum WeaponType
    {
        //步枪
        Rifle,

        //冲锋枪
        SubmachineGun,

        //狙击枪
        SniperRifle,

        //霰弹枪
        ShotGun,

        //弓
        Bow,

        //手枪
        Pistol
    }

    //枪械开火类型
    [EnumToggleButtons]
    public enum FireType
    {
        //单发，三连发，全自动
        SingleFire,
        TripleFire,
        FullyAutoFire
    }

    //武器元素类型
    public enum WeaponElementType
    {
        //极，五行元素均衡，
        PureEquilibrium,

        //金木水火土
        Gold,
        Wood,
        Water,
        Flame,
        Soil,
    }

    //主摄像头


    //武器物体
    public GameObject weaponRoot;

    //枪口闪光，每一把枪械都会有，但是不一定会有弹道,但是近战武器没有枪火闪光,不引用即可
    //枪口闪光的位置
    public Transform weaponMuzzle;

    //枪火闪光的预制体
    public GameObject muzzleFlashPrefab;

    //子弹弹道产生的位置
    public Transform projectilePosition;

    //子弹的预制体
    public ProjectileBaseThomos projectilePrefab;

    //子弹百秒射击量/射速
    public float HundredSecondsShootNumber;

    public float intervalBetweenShots;

    public float _lastShotTime;


    //子弹的射击类型
    public FireType fireType;


    //枪械是否开启显示
    public bool isWeaponActive;
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }


    private void Awake()
    {
        intervalBetweenShots = 100 / HundredSecondsShootNumber;
    }


    public bool HandleShootInput(bool inputHeld)
    {
        if (inputHeld)
        {
            return TryShoot();
        }

        return false;
    }

    private bool TryShoot()
    {
        if (_lastShotTime + intervalBetweenShots < Time.time)
        {
            HandleShoot();
            print("shoot");
            return true;
        }

        return false;
    }

    private void HandleShoot()
    {
        if (fireType is FireType.SingleFire or FireType.FullyAutoFire)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                    weaponMuzzle.rotation, weaponMuzzle.transform);
                Destroy(muzzleFlashInstance, 2f);
            }

            if (projectilePrefab != null)
            {
                Vector3 shotDirection = weaponMuzzle.forward;
                ProjectileBaseThomos newProjectile = Instantiate(projectilePrefab, projectilePosition.position,
                    projectilePosition.rotation, projectilePosition.transform);
                newProjectile.Shoot(this);
            }
        }
        else if (fireType == FireType.TripleFire)
        {
            StartCoroutine(TripleFireTimeInterval());
        }


        _lastShotTime = Time.time;
    }

    IEnumerator TripleFireTimeInterval()
    {
        for (int i = 0; i < 3; i++)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                    weaponMuzzle.rotation, weaponMuzzle.transform);
                Destroy(muzzleFlashInstance, 2f);
            }

            if (projectilePrefab != null)
            {
                Vector3 shotDirection = weaponMuzzle.forward;
                ProjectileBaseThomos newProjectile = Instantiate(projectilePrefab, projectilePosition.position,
                    projectilePosition.rotation, projectilePosition.transform);
                newProjectile.Shoot(this);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }


    //显示武器
    public void ShowWeapon()
    {
        weaponRoot.SetActive(isWeaponActive);
    }

    //todo:切换武器隐藏
    //收起武器，武器在换枪动画结束的关键帧隐藏
    public void HideWeapon()
    {
        weaponRoot.SetActive(isWeaponActive);
    }
}
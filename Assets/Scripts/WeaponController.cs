using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponController : MonoBehaviour
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

    public GameObject weaponRoot;
    public Transform weaponMuzzle;
    public Transform projectilePosition;
    public ProjectileBaseThomos projectilePrefab;
    public GameObject muzzleFlashPrefab;
    //子弹百秒射击量/射速
    public float HundredSecondsShootNumber = 1000f;
    public float intervalBetweenShots;
    public FireType fireType = FireType.SingleFire;

    private float _lastShotTime = Mathf.NegativeInfinity;
    public bool IsWeaponActive { get; set; }
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }


    private void Awake()
    {
        intervalBetweenShots = 100 / HundredSecondsShootNumber;
    }

    //显示武器
    public void ShowWeapon(bool show)
    {
        weaponRoot.SetActive(show);
        IsWeaponActive = show;
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

            yield return new WaitForSeconds(0.1f);

        }
    }
}
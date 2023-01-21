using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;


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
    public Transform weaponShootCheckPoint;

    //十字准心目标距离
    public float crossHairTargetDistance;

    //射线子弹半径
    public float rayProjectileRadius = 0.2f;

    //射线检测距离
    public float maxRaycastDistance = 300f;

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

    //射线命中碰撞体的弹孔，弹道产生的弹孔由子弹负责执行
    public GameObject rayBulletHole;

    //射线命中碰撞体的闪光，弹道产生的闪光由子弹负责执行
    public GameObject rayImpactVFX;

    //射线命中碰撞体的闪光的产生偏移量，不会直接在碰撞体表面
    public float rayImpactVFXSpawnOffset = 0.1f;

    //射线命中碰撞体的弹孔和闪光存在时间
    public float rayImpactVFXAndBulletHoleLifeTime = 5f;

    //子弹百秒射击量/射速
    public float hundredSecondsShootNumber = 1000f;

    //射击时间间隔
    public float intervalBetweenShots;

    //上一次射击的时间
    public float lastShotTime;


    //子弹的射击类型
    public FireType fireType;

    //武器类型
    public WeaponType weapontype;

    //武器元素类型
    public WeaponElementType elementType;


    //枪械是否开启显示
    public bool isWeaponActive;
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }


    private void Awake()
    {
        intervalBetweenShots = 100 / hundredSecondsShootNumber;
        Debug.Log(intervalBetweenShots);
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
        if (lastShotTime + intervalBetweenShots < Time.time)
        {
            switch (weapontype)
            {
                case WeaponType.Rifle:
                    if (RaycastTargetDistance() < 10)
                    {
                        RaycastShoot();
                    }
                    else
                    {
                        TrajectoryShoot();
                    }

                    break;
                case WeaponType.SubmachineGun:
                    break;
                //狙击步枪直接使用射线射击
                case WeaponType.SniperRifle:
                    RaycastShoot();
                    break;
                case WeaponType.ShotGun:
                    break;
                case WeaponType.Bow:
                    break;
                case WeaponType.Pistol:
                    if (RaycastTargetDistance() < 10)
                    {
                        RaycastShoot();
                    }
                    else
                    {
                        TrajectoryShoot();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            print("shoot");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 准心瞄准目标的距离，如果小于10米就不用弹道
    /// </summary>
    /// <returns></returns>
    private float RaycastTargetDistance()
    {
        if (weaponShootCheckPoint != null)
        {
            RaycastHit closeCheckHit = new RaycastHit
            {
                //将新建的这个碰撞成员的距离设置为无限大，因此第一个碰撞成员的距离必定小于它
                distance = Mathf.Infinity
            };
            //todo:射线碰撞检测层,最后场景的布置完成后需要处理,所有射线都要
            //射线检测收集所有碰撞体
            RaycastHit[] checkHit = Physics.RaycastAll(weaponShootCheckPoint.position, weaponShootCheckPoint.forward,
                maxRaycastDistance, -1,
                QueryTriggerInteraction.Ignore);
            //如果收集的碰撞体数组不为空，我才遍历所有成员信息
            if (checkHit != null)
            {
                //遍历碰撞体成员信息
                foreach (var hit in checkHit)
                {
                    //如果检测到的碰撞体信息的标签不为Player，且这个距离小于目前的最小碰撞成员的距离，就让这个成员变为最近的碰撞成员
                    if (!hit.transform.CompareTag("Player") && hit.distance < closeCheckHit.distance)
                    {
                        closeCheckHit = hit;
                    }
                }

                crossHairTargetDistance = closeCheckHit.distance;
            }
            //如果数组为空，说明射线路径上没有碰撞体，所以我们要执行弹道射击，所以准心目标距离要大于5m，100是一个随意设置的值
            else
            {
                crossHairTargetDistance = 100f;
            }
        }

        return crossHairTargetDistance;
    }

    /// <summary>
    /// 实现射线检测攻击的逻辑
    /// </summary>
    private void RaycastShoot()
    {
        switch (fireType)
        {
            case FireType.SingleFire or FireType.FullyAutoFire:
            {
                //枪口闪光的产生
                MuzzleFlashPlay();

                //依旧使用数组来判断是否产生碰撞
                RaycastHit closeShootHit = new RaycastHit
                {
                    distance = Mathf.Infinity
                };
                RaycastHit[] shootHit = Physics.RaycastAll(weaponShootCheckPoint.position,
                    weaponShootCheckPoint.forward,
                    maxRaycastDistance, -1,
                    QueryTriggerInteraction.Ignore);
                //在这个函数之前已经检测过一次，是否有碰撞体，所以这里不用判空，直接遍历所有成员信息
                foreach (var hit in shootHit)
                {
                    //如果检测到的碰撞体信息的标签不为Player，且这个距离小于目前的最小碰撞成员的距离，就让这个成员变为最近的碰撞成员
                    if (!hit.transform.CompareTag("Player") && hit.distance < closeShootHit.distance)
                    {
                        closeShootHit = hit;
                    }
                }

                OnRaySphereHit(closeShootHit.point, closeShootHit.normal);
                break;
            }
            case FireType.TripleFire:
                StartCoroutine(RayTripleFireTimeInterval());
                break;
        }
    }

    /// <summary>
    /// 实现攻击碰撞效果的逻辑
    /// </summary>
    /// <param name="point"></param>
    /// <param name="normal"></param>
    private void OnRaySphereHit(Vector3 point, Vector3 normal)
    {
        if (rayImpactVFX != null && rayBulletHole != null)
        {
            GameObject bulletHoleInstance = Instantiate(rayBulletHole, point, Quaternion.LookRotation(normal));
            GameObject impactVFXInstance = Instantiate(rayImpactVFX, point + normal * rayImpactVFXSpawnOffset,
                Quaternion.LookRotation(normal));
            if (rayImpactVFXAndBulletHoleLifeTime > 0)
            {
                Destroy(bulletHoleInstance, rayImpactVFXAndBulletHoleLifeTime);
                Destroy(impactVFXInstance, rayImpactVFXAndBulletHoleLifeTime);
            }
        }

        print("Ray!Hit!");
    }

    /// <summary>
    /// 实现弹道攻击的逻辑
    /// </summary>
    private void TrajectoryShoot()
    {
        switch (fireType)
        {
            case FireType.SingleFire or FireType.FullyAutoFire:
            {
                MuzzleFlashPlay();

                if (projectilePrefab != null)
                {
                    Vector3 shotDirection = weaponMuzzle.forward;
                    ProjectileBaseThomos newProjectile = Instantiate(projectilePrefab, projectilePosition.position,
                        projectilePosition.rotation, projectilePosition.transform);
                    newProjectile.Shoot(this);
                }

                break;
            }
            case FireType.TripleFire:
                StartCoroutine(TripleFireTimeInterval());
                break;
        }


        lastShotTime = Time.time;
    }

    /// <summary>
    /// 三连发的弹道射击协程
    /// </summary>
    /// <returns></returns>
    IEnumerator TripleFireTimeInterval()
    {
        for (int i = 0; i < 3; i++)
        {
            MuzzleFlashPlay();

            if (projectilePrefab != null)
            {
                //Vector3 shotDirection = weaponMuzzle.forward;
                ProjectileBaseThomos newProjectile = Instantiate(projectilePrefab, projectilePosition.position,
                    projectilePosition.rotation, projectilePosition.transform);
                newProjectile.Shoot(this);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 三连发的射线射击协程
    /// </summary>
    /// <returns></returns>
    IEnumerator RayTripleFireTimeInterval()
    {
        for (int i = 0; i < 3; i++)
        {
            MuzzleFlashPlay();

            Physics.SphereCast(weaponShootCheckPoint.position, rayProjectileRadius, Vector3.forward, out var hit,
                maxRaycastDistance, -1,
                QueryTriggerInteraction.Ignore);
            OnRaySphereHit(hit.point, hit.normal);

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

    //由于闪光使用较多，且闪光的参数不必经常修改，枪火闪光的逻辑过程
    private void MuzzleFlashPlay()
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                weaponMuzzle.rotation, weaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2f);
        }
    }
}
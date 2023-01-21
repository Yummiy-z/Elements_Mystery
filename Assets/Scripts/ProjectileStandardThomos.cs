using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandardThomos : MonoBehaviour
{
    public float maxLifeTime = 5f;
    public float speed = 100f;


    public Transform root;
    public Transform tip;
    public float radius = 0.1f;
    public LayerMask hittableLayers = -1;

    //Imapct VFX
    public GameObject impactVFX;
    public float impactVFXLifeTime = 5f;
    public float impactVFXSpawnOffset = 0.1f;
    public GameObject bulletHole;

    //轨迹修正距离
    public float trajectoryCorrectionDistance = 10;

    private ProjectileBaseThomos _projectileBaseThomos;
    private Vector3 _velocity;

    private Vector3 _lastRootPosition;

    //是否已经完成修正
    private bool _hasTrajectoryCorrected;

    //最终要修正的向量
    private Vector3 _correctedVector3;

    private Vector3 _consumedCorrectionVector3;

    private void OnEnable()
    {
        _projectileBaseThomos = GetComponent<ProjectileBaseThomos>();
        _projectileBaseThomos.OnShoot += StartShoot;
        Destroy(gameObject, maxLifeTime);
    }

    //等同On shoot
    private void StartShoot()
    {
        _lastRootPosition = root.position;
        _velocity += transform.forward * speed;
        PlayerWeaponManager playerWeaponManager = _projectileBaseThomos.Owner.GetComponent<PlayerWeaponManager>();
        if (playerWeaponManager)
        {
            _hasTrajectoryCorrected = false;
            Transform weaponCameraTransform = playerWeaponManager.weaponCamera.transform;

            Vector3 cameraTomuzzle = _projectileBaseThomos.InitialPosition - weaponCameraTransform.position;
            
            _correctedVector3 = Vector3.ProjectOnPlane(-cameraTomuzzle, weaponCameraTransform.forward);
            
        }
    }

    private void Update()
    {
        //Move/子弹移动
        transform.position += _velocity * Time.deltaTime;

        //Drift the projectile to the camera center
        if (!_hasTrajectoryCorrected && _consumedCorrectionVector3.sqrMagnitude < _correctedVector3.sqrMagnitude)
        {
            //Has much correction vector left to be consumed for accuracy adjustment
            //还有很多校正向量需要用于精度调整 correction vector left 左校正向量
            Vector3 correctionVectorLeft = _correctedVector3 - _consumedCorrectionVector3;
            //每一帧移动的长度，在子弹产生第一帧就是速度，后面会多出一个向左偏移量，因为子弹的运动在偏移之前，所以第一帧的位移是没有偏移的
            float distanceThisFrame = (root.position - _lastRootPosition).magnitude;
            /*
             * 从枪口到准心，需要向左偏移以及向上偏移（一般枪口位于准心的右下方）
             * 首先简化问题，单独计算向左偏移，（因为向上偏移和向左偏移本质运算过程一样）
             * 目前给出一个偏移距离distance，5m，即在五米的位置完成偏移，根据子弹创建的位置以及相机的位置得出需要偏移的量Vector3.偏移量
             * 而子弹的速度Velocity.z，单位m/s，是变化的（人为设置子弹速度），所以Velocity是未知的，
             * 另外Vector3.偏移量已知且固定，所以时间Time，是未知的
             * Time=distance/Velocity.z,(second)
             * Vector3.每秒偏移量=Vector3.偏移量/Time=Vector3.偏移量  *  Velocity.z  /  distance
             * 代码为  Velocity.z= `distanceThisFrame` ||   distance=`trajectoryCorrectionDistance
             * 然后再思考向上偏移，而我们运算时是Update，是每帧偏移一次，所以每帧偏移量，就是速度，
             * 所以完整速度Velocity=这一帧的位置减去上一帧子弹的位置，这个每帧偏移量包括了向上和向左两个方向的偏移。
             * float distanceThisFrame = (root.position - _lastRootPosition).magnitude;
             * Vector3 correctionThisFrame =(distanceThisFrame / trajectoryCorrectionDistance) * _correctedVector3;
             */
            //Unity官方案例
            Vector3 correctionThisFrame = (distanceThisFrame / trajectoryCorrectionDistance) * _correctedVector3;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionVectorLeft.magnitude);
            _consumedCorrectionVector3 += correctionThisFrame;
            if (Mathf.Abs(_consumedCorrectionVector3.sqrMagnitude - _correctedVector3.sqrMagnitude) < Mathf.Epsilon)
            {
                _hasTrajectoryCorrected = true;
            }

            //Start drifting the projectile
            transform.position += correctionThisFrame;
        }

        //Orient
        transform.forward = _velocity.normalized;

        //Hit detection
        RaycastHit closestHit = new RaycastHit
        {
            distance = Mathf.Infinity
        };
        var foundHit = false;

        //SphereCastAll
        Vector3 displacementsSinceLastFrame = tip.position - _lastRootPosition;
        
        RaycastHit[] hits = Physics.SphereCastAll(_lastRootPosition, radius, displacementsSinceLastFrame.normalized,
            displacementsSinceLastFrame.magnitude, hittableLayers, QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits)
        {
            if (IsHitValid(hit) && hit.distance < closestHit.distance)
            {
                closestHit = hit;
                foundHit = true;
            }
        }

        if (foundHit)
        {
            if (closestHit.distance <= 0)
            {
                closestHit.point = root.position;
                closestHit.normal = -transform.forward;
            }

            Debug.Log("closestHit.point:" + closestHit.point);
            OnHit(closestHit.point, closestHit.normal);
        }

        _lastRootPosition = root.position;
    }


    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.isTrigger)
        {
            return false;
        }

        return true;
    }

    private void OnHit(Vector3 point, Vector3 normal)
    {
        if (impactVFX != null && bulletHole != null)
        {
            GameObject bulletHoleInstance = Instantiate(bulletHole, point, Quaternion.LookRotation(normal));
            GameObject impactVFXInstance = Instantiate(impactVFX, point + normal * impactVFXSpawnOffset,
                Quaternion.LookRotation(normal));
            if (impactVFXLifeTime > 0)
            {
                Destroy(bulletHoleInstance, impactVFXLifeTime);
                Destroy(impactVFXInstance, impactVFXLifeTime);
            }
        }

        print("Hit");
        Destroy(gameObject);
    }
}
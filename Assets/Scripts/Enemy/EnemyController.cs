using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUAD,
    PATROL,
    CHASE,
    ATTACK,
    DEAD
}

public class EnemyController : MonoBehaviour
{
    //敌人状态
    public EnemyStates enemyState;

    //敌人数据
    public EnemyStats enemyStats;

    //寻找玩家半径
    public float slightRadius;

    //攻击判定范围
    public BoxCollider attackAreaCube;
    private const float CheckAttackRadius = 1f;

    private Animator _anim;
    private NavMeshAgent _agent;
    private Transform _targetPlayer;
    [SerializeField] private bool enterAttackState;

    private bool _isWalk;

    //攻击状态的前摇
    private float _preAttackSway = 0.5f;

    //攻击状态的间隔时间
    private float _attackInterval = 5f;
    private float _lastAttackTime;


    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        _agent = GetComponent<NavMeshAgent>();
        attackAreaCube = GetComponent<BoxCollider>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        CheckAndSwitchStates();
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        _anim.SetBool("IsWalk", _isWalk);
    }

    private void CheckAndSwitchStates()
    {
        if (IsFoundPlayer() && !CheckCanAttack())
        {
            enemyState = EnemyStates.CHASE;
        }
        else if (!CheckCanAttack())
        {
            enemyState = EnemyStates.GUAD;
        }

        if (CheckCanAttack())
        {
            enemyState = EnemyStates.ATTACK;
        }

        switch (enemyState)
        {
            case EnemyStates.GUAD:

                _agent.isStopped = true;
                _isWalk = false;
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                _lastAttackTime = 0f;
                _isWalk = true;
                enterAttackState = false;
                _agent.isStopped = false;
                _agent.SetDestination(_targetPlayer.position);
                //Debug.Log("targetPlayer.position" + _targetPlayer.position);
                break;
            case EnemyStates.ATTACK:

                _agent.isStopped = true;
                _isWalk = false;
                Attack();

                break;
            case EnemyStates.DEAD:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Attack()
    {
        if (_lastAttackTime <= 0)
        {
            _agent.transform.LookAt(_targetPlayer);
            //对准玩家后开始播放动画
            _anim.SetTrigger("Attack");
            Debug.Log("开始造成伤害");
            //_lastAttackTime = enemyStats.enemyAttackData.coolDown;
            _lastAttackTime = _attackInterval;
        }

        _lastAttackTime -= Time.deltaTime;
    }

    private bool CheckCanAttack()
    {
        //todo:检测方式可能需要性能优化修改
        var CheckCanAttack = Physics.SphereCastAll(transform.position, CheckAttackRadius,
            transform.forward,
            _agent.radius + enemyStats.AttackRange,
            LayerMask.GetMask("Player"));
        Debug.DrawRay(transform.position, transform.forward * (_agent.radius + enemyStats.AttackRange), Color.cyan,
            3f);
        foreach (var collider in CheckCanAttack)
        {
            if (collider.transform.CompareTag("Player"))
            {
                enterAttackState = true;
                return true;
            }
        }

        return false;
    }


    private bool IsFoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, slightRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                _targetPlayer = collider.transform;
                return true;
            }
        }

        _targetPlayer = null;
        return false;

        // return colliders.Any(col => col.CompareTag("Player"));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, slightRadius);
    }
}
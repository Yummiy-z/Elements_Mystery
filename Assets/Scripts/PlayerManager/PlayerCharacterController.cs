using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacterController : Singleton<PlayerCharacterController>
{
    /// <summary>
    /// 继承工具类，单例模式，一个玩家只需要一个玩家角色控制器脚本
    /// </summary>
    [Header("General & Movement")] public Camera playerCamera;
    [Tooltip("模拟重力")] public float gravityDownForce = 20f;
    public float walkSpeed = 5f;
    public float jumpHeight = 1.5f;
    [Tooltip("Lerp差值计算的反应速度")] public float moveSharpnessOnGround = 15f;
    public float rotationSpeed = 200f;
    [Tooltip("摄像机的身高占比")] public float cameraHeightRatio = 0.9f;

    [Header("CheckGround & 地面检测变量")] [Tooltip("胶囊检测半径")]
    public float distanceToGround = 0.1f;

    public Transform checkGroundPoint;
    [Tooltip("地面检测层")] public LayerMask groundLayer;

    [Header("Sprint/冲刺")] [Tooltip("计算过程中的临时速度")]
    public float temporarySpeed;

    [Tooltip("冲刺速度")] public float sprintSpeed = 25f;
    public float sprintDistance = 3f;
    public int maxSprintNumbers = 1;
    public float coolDown = 5f;
    [SerializeField]private int _remainingSprintNumbers = 1;
    private float _lastSprintStartTime = 0;

    [Header("State/状态判定")] [Tooltip("地面检测")] [SerializeField]
    private bool characterIsGrounded;

    [Tooltip("是否冲刺过程")] [SerializeField] private bool characterIsSprinting;

    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private float _targetCharacterHeight = 2f;
    private float _cameraVerticalAngle;
    private Vector3 _startSprintPosition = Vector3.zero;
    private Vector3 _closeSprintPosition;

    public Vector3 CharacterVelocity { get; set; }

    public Vector3 JumpVelocity { get; set; }


    //测试语句，复制使用：Debug.Log("checkGroundPoint.position:" + checkGroundPoint.position);
    

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        //防止角色控制器和墙体持续重叠卡住
        _characterController.enableOverlapRecovery = true;
        //初始化角色高度和摄像机高度
        UpdateCharacterHeight();
        //赋值行走速度给临时变量
        temporarySpeed = walkSpeed;
    }

    private void Update()
    {
        HandleCharacterMovement();
        SprintCoolDownRefreshCheck();
    }


    private void UpdateCharacterHeight()
    {
        _characterController.height = _targetCharacterHeight;
        //_characterController.center = Vector3.up * _characterController.height * 0.5f;
        //todo:相机位置可能需要调整
        playerCamera.transform.localPosition = Vector3.up *
                                               (_characterController.height -
                                                _characterController.transform.position.y) * cameraHeightRatio;
    }

    // ReSharper restore Unity.ExpensiveCode

    private void HandleCharacterMovement()
    {
        // Camera rotate horizontal
        // 实际是身体转向
        transform.Rotate(new Vector3(0, _inputHandler.GetMouseLookHorizontal() * rotationSpeed, 0),
            Space.Self);
        //-------------------------------------------------------
        // Camera rotate vertical
        _cameraVerticalAngle += _inputHandler.GetMouseLookVertical() * rotationSpeed;

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(-_cameraVerticalAngle, 0, 0);
        //----------------------------------------------------
        //SprintCheck
        if (CharacterVelocity != Vector3.zero && _inputHandler.GetSprintInput() && characterIsSprinting == false &&
            _remainingSprintNumbers > 0)
        {
            characterIsSprinting = true;
            _remainingSprintNumbers--;
            _lastSprintStartTime = Time.time;
            //开始进入冲刺过程
            StartSprint();
        }

        if (characterIsSprinting)
        {
            CheckSprintFinished();
        }

        // Move 
        Vector3 worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());
        Vector3 targetVelocity = worldSpaceMoveInput * walkSpeed;
        CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
        //差值计算
        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
            moveSharpnessOnGround * Time.deltaTime);
        _characterController.Move(CharacterVelocity * Time.deltaTime);
        //-----------------------------------------------------
        //Jump
        CheckGround();
        if (characterIsGrounded)
        {
            JumpVelocity = Vector3.zero;
            if (characterIsGrounded && _inputHandler.GetJumpInput())
            {
                JumpVelocity = new Vector3(JumpVelocity.x, 0f, JumpVelocity.z);
                //Velocity = 2gh 开平方根
                JumpVelocity += Vector3.up * Mathf.Sqrt(2f * gravityDownForce * jumpHeight);
                _characterController.Move(JumpVelocity * Time.deltaTime);
            }
        }
        else
        {
            JumpVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        _characterController.Move(JumpVelocity * Time.deltaTime);
    }

    private void CheckGround()
    {
        var checkPosition = checkGroundPoint.position;
        Vector3 capsuleBottom = new Vector3(checkPosition.x,
            checkPosition.y - 0.5f, checkPosition.z);
        characterIsGrounded = Physics.CheckCapsule(checkPosition, capsuleBottom, distanceToGround,
            groundLayer,
            QueryTriggerInteraction.Ignore);
    }

    //进入冲刺
    private void StartSprint()
    {
        _startSprintPosition = _characterController.transform.position;
        walkSpeed = sprintSpeed;
    }

    //根据冲刺距离判定冲刺是否结束
    private void CheckSprintFinished()
    {
        _closeSprintPosition = _characterController.transform.position;

        //sqrMagnitude is faster than magnitude. (computing squared magnitudes is faster). 计算平方比开根号快
        if ((_closeSprintPosition - _startSprintPosition).sqrMagnitude <= sprintDistance * sprintDistance) return;
        walkSpeed = temporarySpeed;
        characterIsSprinting = false;
        _startSprintPosition = Vector3.zero;
    }

    //冲刺技能CD刷新
    private void SprintCoolDownRefreshCheck()
    {
        if (_remainingSprintNumbers < maxSprintNumbers && _lastSprintStartTime + coolDown < Time.time)
        {
            _remainingSprintNumbers++;
        }
    }
}
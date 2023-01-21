
using UnityEngine;


public class PlayerCharacterController : MonoBehaviour
{
    private PlayerBaseData _playerBaseData;
    [Header("General & Movement")] public Camera playerCamera;

    private const float GravityDownForce = 20f;
    //[SerializeField] private float walkSpeed = 5f;
    //private float jumpHeight = 1.5f;

    [SerializeField] [Tooltip("前进速度的Lerp差值计算的反应速率")]
    private float moveSharpnessOnGround = 15f;

    private const float RotationSpeed = 200f;
    [SerializeField] [Tooltip("摄像机的身高占比")] private float cameraHeightRatio = 0.9f;

    [Header("CheckGround & CheckSlope")] [SerializeField] [Tooltip("球形检测半径")]
    private float distanceToGround = 0.6f;

    public Transform checkGroundAndSlopePoint;
    [SerializeField] [Tooltip("地面检测层")] private LayerMask groundLayer;

    private const float SlopeRayDistance = 0.8f;

    //如果在斜坡上，给予一个向下的速度，让向前的速度和向下的速度看成一个斜向下的速度
    private Vector3 _onSlopeDownSpeed = Vector3.zero;

    [Header("Sprint/冲刺")] [Tooltip("计算过程中的临时速度")]
    private float _temporarySpeed;

    private const float SprintSpeed = 25f;

     //private float sprintDistance = 3f;
    //private int _maxSprintNumbers = 1;
    //private float _sprintCoolDown = 5f;

    [SerializeField] [Tooltip("剩余的冲刺次数")] private int remainingSprintNumbers = 1;
    private float _lastSprintStartTime ;

    [Header("State/状态判定")] [Tooltip("地面检测")] [SerializeField]
    private bool characterIsGrounded;

    [Tooltip("是否冲刺过程")] [SerializeField] private bool characterIsSprinting;

    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private readonly float _targetCharacterHeight = 2f;
    private float _cameraVerticalAngle;
    private Vector3 _startSprintPosition = Vector3.zero;
    private Vector3 _closeSprintPosition;

    private Vector3 _playerForwardVelocity;

    private Vector3 _jumpVelocity;


    //测试语句，复制使用：Debug.Log("checkGroundAndSlopePoint.position:" + checkGroundAndSlopePoint.position);

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _playerBaseData = GetComponent<PlayerBaseData>();
    }

    private void Start()
    {
        //防止角色控制器和墙体持续重叠卡住
        _characterController.enableOverlapRecovery = true;
        //初始化角色高度和摄像机高度
        UpdateCharacterHeight();
        //赋值行走速度给临时变量
        _temporarySpeed = _playerBaseData.WalkSpeed;
    }

    private void Update()
    {
        HandleCharacterMovement();
        SprintCoolDownRefreshCheck();
    }

    private bool OnSlopeCheck()
    {
        if (Physics.Raycast(checkGroundAndSlopePoint.position, Vector3.down, out var hit,
                SlopeRayDistance, groundLayer) && hit.normal != Vector3.up)
        {
            return true;
        }

        return false;
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
        transform.Rotate(new Vector3(0, _inputHandler.GetMouseLookHorizontal() * RotationSpeed, 0),
            Space.Self);
        //-------------------------------------------------------
        // Camera rotate vertical
        _cameraVerticalAngle += _inputHandler.GetMouseLookVertical() * RotationSpeed;

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(-_cameraVerticalAngle, 0, 0);
        //----------------------------------------------------
        //SprintCheck
        if (_playerForwardVelocity != Vector3.zero && _inputHandler.GetSprintInput() && characterIsSprinting == false &&
            remainingSprintNumbers > 0)
        {
            characterIsSprinting = true;
            remainingSprintNumbers--;
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
        Vector3 targetVelocity = worldSpaceMoveInput * _playerBaseData.WalkSpeed;
        _playerForwardVelocity = new Vector3(_playerForwardVelocity.x, 0f, _playerForwardVelocity.z);
        //差值计算
        _playerForwardVelocity = Vector3.Lerp(_playerForwardVelocity, targetVelocity,
            moveSharpnessOnGround * Time.deltaTime);
        _characterController.Move(_playerForwardVelocity * Time.deltaTime);
        //-----------------------------------------------------
        //Jump
        CheckGround();
        if (characterIsGrounded)
        {
            _jumpVelocity = Vector3.zero;
            if (characterIsGrounded && _inputHandler.GetJumpInput())
            {
                _jumpVelocity = new Vector3(_jumpVelocity.x, 0f, _jumpVelocity.z);
                //Velocity = 2gh 开平方根
                _jumpVelocity += Vector3.up * Mathf.Sqrt(2f * GravityDownForce * _playerBaseData.JumpHeight);
                _characterController.Move(_jumpVelocity * Time.deltaTime);
            }
        }
        else
        {
            _jumpVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
        }

        if (OnSlopeCheck())
        {
            _onSlopeDownSpeed = Vector3.down * 2f;
        }

        _characterController.Move((_jumpVelocity + _onSlopeDownSpeed) * Time.deltaTime);
    }

    private void CheckGround()
    {
        var checkPosition = checkGroundAndSlopePoint.position;
        characterIsGrounded = Physics.CheckSphere(checkPosition, distanceToGround,
            groundLayer,
            QueryTriggerInteraction.Ignore);
    }

    //进入冲刺
    private void StartSprint()
    {
        //todo：播放冲刺动画（如果联机需要）
        _startSprintPosition = _characterController.transform.position;
        _playerBaseData.WalkSpeed = SprintSpeed;
    }

    //根据冲刺距离判定冲刺是否结束
    private void CheckSprintFinished()
    {
        _closeSprintPosition = _characterController.transform.position;
        //sqrMagnitude is faster than magnitude. (computing squared magnitudes is faster). 计算平方比开根号快
        if ((_closeSprintPosition - _startSprintPosition).sqrMagnitude <=
            _playerBaseData.SprintDistance * _playerBaseData.SprintDistance) return;
        _playerBaseData.WalkSpeed = _temporarySpeed;
        characterIsSprinting = false;
        _startSprintPosition = Vector3.zero;
    }

    //冲刺技能CD刷新
    private void SprintCoolDownRefreshCheck()
    {
        if (remainingSprintNumbers < _playerBaseData.MaxSprintNumbers &&
            _lastSprintStartTime + _playerBaseData.SprintCoolDown < Time.time)
        {
            remainingSprintNumbers++;
        }
    }
}
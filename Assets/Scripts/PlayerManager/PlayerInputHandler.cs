using UnityEngine;


public class PlayerInputHandler : Singleton<PlayerInputHandler>
{
    //继承工具类，单例模式，一个玩家只会有一个玩家输入处理脚本

    //转向灵敏度，可以修改该值来变化视角旋转速度
    [Tooltip("转向灵敏度")] public float lookSensitivity = 1f;


    private void Start()
    {
        //鼠标锁定以及鼠标隐形
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //移动输入采集
    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }

    //冲刺输入采集
    public bool GetSprintInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    //跳跃输入采集
    public bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    //通过鼠标位移采集方法采集X，即视角横向
    public float GetMouseLookHorizontal()
    {
        return GetMouseLookAxis("Mouse X");
    }

    //通过鼠标位移采集方法采集Y，即视角纵向
    public float GetMouseLookVertical()
    {
        return GetMouseLookAxis("Mouse Y");
    }

    //鼠标位移采集方法
    private float GetMouseLookAxis(string mouseInputName)
    {
        var input = Input.GetAxisRaw(mouseInputName);
        input *= lookSensitivity * 0.01f;

        return input;
    }

    //子弹射击连发输入
    public bool GetKeepFiringInput()
    {
        return Input.GetButton("Fire1");
    }

    //子弹射击单发输入
    public bool GetSingleFireInput()
    {
        return Input.GetButtonDown("Fire1");
    }

    //是否出现切换武器指令
    public bool SwitchWeaponInput()
    {
        return Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3);
    }

    //要切换到的武器的序号采集
    public int SwitchWeaponNumberInput()
    {
        //todo:同时按键不能响应的逻辑没有完成
        //此功能好像不需要，就算肉眼同时按，电脑检测还是会有区别
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("按键1");
            return 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("按键2");
            return 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("按键3");
            return 3;
        }

        return 0;
    }
}
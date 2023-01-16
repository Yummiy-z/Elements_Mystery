using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    //单例模式，一个玩家只会有一个玩家输入处理脚本
    public static PlayerInputHandler Instance;

    //转向灵敏度，可以修改该值来变化视角旋转速度
    [Tooltip("转向灵敏度")] public float lookSensitivity = 1f;

    private void Awake()
    {
        Instance = this;
    }

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
}
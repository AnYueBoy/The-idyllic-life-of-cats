using System;
using UnityEngine;

public class InputManager : IManager
{
    public void Init()
    {
        isRun = false;
        roleDirection = RoleDirection.None;
    }

    public void LocalUpdate(float dt)
    {
        CheckInput();
    }

    public event Action<RoleDirection> idleEvent;
    public event Action<RoleDirection, bool> moveEvent;

    private bool isRun;
    private RoleDirection roleDirection;

    private void CheckInput()
    {
        RoleIdleInput();
        RoleRunInput();
        RoleMoveInput();
    }

    #region 键盘输入

    private void RoleIdleInput()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            idleEvent?.Invoke(RoleDirection.Left);
            roleDirection = RoleDirection.None;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            idleEvent?.Invoke(RoleDirection.Right);
            roleDirection = RoleDirection.None;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            idleEvent?.Invoke(RoleDirection.Positive);
            roleDirection = RoleDirection.None;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            idleEvent?.Invoke(RoleDirection.Back);
            roleDirection = RoleDirection.None;
        }
    }

    private void RoleMoveInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveEvent?.Invoke(RoleDirection.Left, isRun);
            roleDirection = RoleDirection.Left;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            moveEvent?.Invoke(RoleDirection.Right, isRun);
            roleDirection = RoleDirection.Right;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            moveEvent?.Invoke(RoleDirection.Positive, isRun);
            roleDirection = RoleDirection.Positive;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            moveEvent?.Invoke(RoleDirection.Back, isRun);
            roleDirection = RoleDirection.Back;
        }
    }

    private void RoleRunInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRun = true;
            SwitchRunState();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun = false;
            SwitchRunState();
        }
    }

    private void SwitchRunState()
    {
        if (roleDirection == RoleDirection.Positive)
        {
            moveEvent?.Invoke(RoleDirection.Positive, isRun);
        }

        if (roleDirection == RoleDirection.Back)
        {
            moveEvent?.Invoke(RoleDirection.Back, isRun);
        }

        if (roleDirection == RoleDirection.Left)
        {
            moveEvent?.Invoke(RoleDirection.Left, isRun);
        }

        if (roleDirection == RoleDirection.Right)
        {
            moveEvent?.Invoke(RoleDirection.Right, isRun);
        }
    }

    #endregion

    #region 鼠标输入

    
    #endregion
}
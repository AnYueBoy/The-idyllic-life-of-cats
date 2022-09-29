using System;
using UnityEngine;

public class InputManager : IManager
{
    public void Init()
    {
    }

    public void LocalUpdate(float dt)
    {
        CheckInput();
    }

    public event Action<RoleDirection> idleEvent;
    public event Action<RoleDirection> moveEvent;

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            idleEvent?.Invoke(RoleDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            idleEvent?.Invoke(RoleDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            idleEvent?.Invoke(RoleDirection.Positive);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            idleEvent?.Invoke(RoleDirection.Back);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            moveEvent?.Invoke(RoleDirection.Left);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            moveEvent?.Invoke(RoleDirection.Right);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveEvent?.Invoke(RoleDirection.Positive);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            moveEvent?.Invoke(RoleDirection.Back);
        }
        // TODO: 非编辑器下的移动逻辑
    }
}
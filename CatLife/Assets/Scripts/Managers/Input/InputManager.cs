using System;
using BitFramework.Component.AssetsModule;
using BitFramework.Core;
using UnityEngine;

public class InputManager : IManager
{
    public void Init()
    {
        isRun = false;
        roleDirection = RoleDirection.None;
        CreateMouseNode();
    }

    public void LocalUpdate(float dt)
    {
        CheckKeyboardInput();
        CheckMouseInput();
    }

    public event Action<RoleDirection> idleEvent;
    public event Action<RoleDirection, bool> moveEvent;

    private bool isRun;
    private RoleDirection roleDirection;


    #region 键盘输入

    private void CheckKeyboardInput()
    {
        RoleIdleInput();
        RoleRunInput();
        RoleMoveInput();
    }

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

    private void CheckMouseInput()
    {
        MouseMove();
    }

    private void MouseMove()
    {
        var mousePos = Input.mousePosition;
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseRender.transform.position = new Vector3(worldPos.x, worldPos.y, 0);
    }

    #endregion

    private SpriteRenderer mouseRender;

    private void CreateMouseNode()
    {
        mouseRender = new GameObject("MouseRender").AddComponent<SpriteRenderer>();
        mouseRender.transform.SetParent(App.Make<NodeManager>().WorldUITrans);
        mouseRender.transform.position = Vector3.one * 10000;
        var mouseSpriteAssets = App.Make<IAssetsManager>().GetAssetByUrlSync<Sprite>(AssetsPath.MouseArrowPath);
        mouseRender.sprite = mouseSpriteAssets;
    }
}
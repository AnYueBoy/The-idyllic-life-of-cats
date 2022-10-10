using System;
using BitFramework.Component.AssetsModule;
using BitFramework.Core;
using UnityEngine;

public class InputManager : IManager
{
    public void Init()
    {
        CreateMouseNode();
    }

    public void LocalUpdate(float dt)
    {
        CheckKeyboardInput();
        CheckMouseInput();
    }

    public event Action<Vector3> moveEvent;
    public event Action<bool> runEvent;

    #region 键盘输入

    private void CheckKeyboardInput()
    {
        RunInput();
    }

    private void RunInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            runEvent?.Invoke(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            runEvent?.Invoke(false);
        }
    }

    #endregion

    #region 鼠标输入

    private void CheckMouseInput()
    {
        MouseMove();
        MouseDown();
    }

    private void MouseMove()
    {
        var mousePos = Input.mousePosition;
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseRender.transform.position = new Vector3(worldPos.x, worldPos.y, 0);
    }

    private Vector3 targetPos;

    private void MouseDown()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var mousePos = Input.mousePosition;
            targetPos = Camera.main.ScreenToWorldPoint(mousePos);
            targetPos.z = 0;
            moveEvent?.Invoke(targetPos);
        }
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
        mouseRender.sortingOrder = SortLayer.WorldUI;
    }
}
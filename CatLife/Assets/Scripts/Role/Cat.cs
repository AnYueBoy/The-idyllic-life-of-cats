using System.Collections.Generic;
using BitFramework.Core;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class Cat : BaseRole
{
    [SerializeField] private List<AnimationClip> idleClipList;
    [SerializeField] private List<AnimationClip> moveClipList;
    [SerializeField] private List<AnimationClip> runClipList;
    private Animator animator;
    private PlayableGraph playableGraph;
    private readonly float moveSpeed = 2f;
    private readonly float runSpeed = 3f;

    public override void Init()
    {
        animator ??= GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingOrder = SortLayer.RoleLayer;
    }

    private void OnEnable()
    {
        // 注册鼠标移动事件
        App.Make<InputManager>().moveEvent += MouseClickCallback;
        App.Make<InputManager>().runEvent += RunKeyCallback;
    }

    public override void LocalUpdate(float dt)
    {
        base.LocalUpdate(dt);
        MouseRoleMoveLogic(dt);
    }

    private RoleDirection roleDirection = RoleDirection.None;

    #region 鼠标移动

    private void MouseRoleMoveLogic(float dt)
    {
        if (movePosList == null || movePosList.Count <= 0 || moveIndex >= movePosList.Count)
        {
            return;
        }

        Vector3 targetPos = movePosList[moveIndex];

        float curMoveSpeed = isRun ? runSpeed : moveSpeed;
        float moveDis = curMoveSpeed * dt;
        float leftMoveDir = (transform.position - targetPos).magnitude;
        if (moveDis > leftMoveDir)
        {
            transform.position = targetPos;
            moveIndex++;
            if (moveIndex >= movePosList.Count)
            {
                RoleIdleAni();
            }
            else
            {
                RefreshDirAndAni();
            }

            return;
        }

        Vector3 nextMovePos = transform.position + moveDir * moveDis;
        transform.position = nextMovePos;
    }

    private Vector3 moveDir;

    private void MouseClickCallback(Vector3 endPos)
    {
        var findPathList = App.Make<MapManager>().FindPath(transform.position, endPos);
        if (findPathList == null)
        {
            Debug.Log("未找到路径");
            return;
        }

        if (findPathList.Count <= 0)
        {
            Debug.Log("原地路径");
            return;
        }

        moveIndex = 0;
        movePosList = findPathList;
        RefreshDirAndAni();
    }

    private void RefreshDirAndAni()
    {
        var targetPos = movePosList[moveIndex];

        // 分解水平垂直移动方向来决定动画移动方向
        Vector3 directVec = targetPos - transform.position;

        RoleDirection curRoleDir;
        // 使用水平方向
        if (Mathf.Abs(directVec.x) > Mathf.Abs(directVec.y))
        {
            curRoleDir = directVec.x > 0 ? RoleDirection.Right : RoleDirection.Left;
        }
        else
        {
            // 使用垂直方向
            curRoleDir = directVec.y > 0 ? RoleDirection.Back : RoleDirection.Positive;
        }

        moveDir = directVec.normalized;

        if (curRoleDir != roleDirection)
        {
            roleDirection = curRoleDir;
            RoleMoveAni();
        }
    }

    private void RunKeyCallback(bool isRun)
    {
        this.isRun = isRun;
        if (roleDirection == RoleDirection.None)
        {
            return;
        }

        RoleMoveAni();
    }

    #endregion

    #region 动画播放

    private bool isRun;

    private void RoleMoveAni()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }

        if (isRun)
        {
            AnimationPlayableUtilities.PlayClip(animator, runClipList[(int)roleDirection], out playableGraph);
        }
        else
        {
            AnimationPlayableUtilities.PlayClip(animator, moveClipList[(int)roleDirection], out playableGraph);
        }
    }

    private void RoleIdleAni()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }

        AnimationPlayableUtilities.PlayClip(animator, idleClipList[(int)roleDirection], out playableGraph);
        roleDirection = RoleDirection.None;
    }

    #endregion

    private void OnDisable()
    {
        App.Make<InputManager>().moveEvent -= MouseClickCallback;
        App.Make<InputManager>().runEvent -= RunKeyCallback;
    }

    private void OnDestroy()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }
    }
}
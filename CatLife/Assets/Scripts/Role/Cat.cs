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
    private readonly float moveSpeed = 3f;
    private readonly float runSpeed = 6f;

    public override void Init()
    {
        animator ??= GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 注册鼠标移动事件
        App.Make<InputManager>().mouseMoveEvent += MouseMoveEnd;
    }

    public override void LocalUpdate(float dt)
    {
        MouseRoleMoveLogic(dt);
    }

    private RoleDirection roleDirection = RoleDirection.None;

    #region 鼠标移动

    private void MouseRoleMoveLogic(float dt)
    {
        if (transform.position.Equals(targetPos))
        {
            return;
        }

        float curMoveSpeed = isRun ? runSpeed : moveSpeed;
        float moveDis = curMoveSpeed * dt;
        float leftMoveDir = (transform.position - targetPos).magnitude;
        if (moveDis > leftMoveDir)
        {
            transform.position = targetPos;
            RoleIdleAni();
            return;
        }

        Vector3 nextMovePos = transform.position + moveDir * moveDis;
        transform.position = nextMovePos;
    }

    private Vector3 targetPos;
    private Vector3 moveDir;

    private void MouseMoveEnd(Vector3 endPos)
    {
        targetPos = endPos;
        // 分解水平垂直移动方向来决定动画移动方向
        Vector3 directVec = targetPos - transform.position;

        // 使用水平方向
        if (Mathf.Abs(directVec.x) > Mathf.Abs(directVec.y))
        {
            roleDirection = directVec.x > 0 ? RoleDirection.Right : RoleDirection.Left;
        }
        else
        {
            // 使用垂直方向
            roleDirection = directVec.y > 0 ? RoleDirection.Back : RoleDirection.Positive;
        }

        moveDir = directVec.normalized;

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
        App.Make<InputManager>().mouseMoveEvent -= MouseMoveEnd;
    }

    private void OnDestroy()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }
    }
}
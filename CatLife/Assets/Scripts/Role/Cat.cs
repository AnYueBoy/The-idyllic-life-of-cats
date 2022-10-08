using System;
using System.Collections.Generic;
using BitFramework.Core;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class Cat : BaseRole
{
    [SerializeField] private List<AnimationClip> idleClipList;
    [SerializeField] private List<AnimationClip> moveClipList;
    private Animator animator;
    private PlayableGraph playableGraph;
    private readonly float moveSpeed = 1.5f;

    public override void Init()
    {
        animator ??= GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 注册相关事件 
        App.Make<InputManager>().idleEvent += RoleIdleAni;
        App.Make<InputManager>().moveEvent += RoleMoveAni;
    }

    public override void LocalUpdate(float dt)
    {
        RoleMoveLogic(dt);
    }

    private RoleDirection roleDirection = RoleDirection.None;

    private void RoleMoveLogic(float dt)
    {
        if (roleDirection == RoleDirection.None)
        {
            return;
        }

        float moveDis = dt * moveSpeed;
        if (roleDirection == RoleDirection.Left)
        {
            transform.localPosition -= new Vector3(moveDis, 0, 0);
        }
        else if (roleDirection == RoleDirection.Right)
        {
            transform.localPosition += new Vector3(moveDis, 0, 0);
        }
        else if (roleDirection == RoleDirection.Back)
        {
            transform.localPosition += new Vector3(0, moveDis, 0);
        }
        else
        {
            transform.localPosition -= new Vector3(0, moveDis, 0);
        }
    }

    private void RoleMoveAni(RoleDirection direction)
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }

        AnimationPlayableUtilities.PlayClip(animator, moveClipList[(int)direction], out playableGraph);
        roleDirection = direction;
    }

    private void RoleIdleAni(RoleDirection direction)
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }

        AnimationPlayableUtilities.PlayClip(animator, idleClipList[(int)direction], out playableGraph);
        roleDirection = RoleDirection.None;
    }

    private void OnDisable()
    {
        App.Make<InputManager>().idleEvent -= RoleIdleAni;
        App.Make<InputManager>().moveEvent -= RoleMoveAni;
    }

    private void OnDestroy()
    {
        playableGraph.Destroy();
    }
}
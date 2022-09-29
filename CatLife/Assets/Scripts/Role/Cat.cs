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
        App.Make<InputManager>().idleEvent += RoleIdle;
        App.Make<InputManager>().moveEvent += RoleMove;
    }

    public override void LocalUpdate(float dt)
    {
    }

    private void RoleMove(RoleDirection direction)
    {
        AnimationPlayableUtilities.PlayClip(animator, moveClipList[(int)direction], out playableGraph);
    }

    private void RoleIdle(RoleDirection direction)
    {
        AnimationPlayableUtilities.PlayClip(animator, idleClipList[(int)direction], out playableGraph);
    }

    private void OnDisable()
    {
        App.Make<InputManager>().idleEvent -= RoleIdle;
        App.Make<InputManager>().moveEvent -= RoleMove;
    }
}

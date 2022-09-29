using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class Role : MonoBehaviour
{
    [SerializeField] private List<AnimationClip> idleClipList;
    [SerializeField] private List<AnimationClip> moveClipList;
    private Animator animator;

    void Start()
    {
        animator ??= GetComponent<Animator>();
    }

    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RoleMove(RoleDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RoleMove(RoleDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RoleMove(RoleDirection.Positive);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RoleMove(RoleDirection.Back);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            RoleIdle(RoleDirection.Left);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            RoleIdle(RoleDirection.Right);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            RoleIdle(RoleDirection.Positive);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            RoleIdle(RoleDirection.Back);
        }
    }

    private PlayableGraph playableGraph;

    private void RoleMove(RoleDirection direction)
    {
        AnimationPlayableUtilities.PlayClip(animator, moveClipList[(int)direction], out playableGraph);
    }

    private void RoleIdle(RoleDirection direction)
    {
        AnimationPlayableUtilities.PlayClip(animator, idleClipList[(int)direction], out playableGraph);
    }
}
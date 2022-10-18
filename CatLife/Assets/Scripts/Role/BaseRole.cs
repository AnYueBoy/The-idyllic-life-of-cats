using System.Collections.Generic;
using BitFramework.Core;
using UnityEngine;

public abstract class BaseRole : MonoBehaviour
{
    public abstract void Init();

    public virtual void LocalUpdate(float dt)
    {
        DrawPath();
    }

    protected List<Vector3> movePosList;
    protected int moveIndex;

    private void DrawPath()
    {
        if (!App.Make<MapManager>().IsOpenDebug || movePosList == null || movePosList.Count < 1 ||
            moveIndex >= movePosList.Count)
        {
            return;
        }

        for (int i = moveIndex; i < movePosList.Count - 1; i++)
        {
            Debug.DrawLine(movePosList[i], movePosList[i + 1], Color.red, -1);
        }
        
        Debug.DrawLine(transform.position, movePosList[moveIndex], Color.red, -1);
    }
}
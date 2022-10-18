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

    private void DrawPath()
    {
        if (!App.Make<MapManager>().IsOpenDebug || movePosList == null || movePosList.Count < 1)
        {
            return;
        }

        for (int i = 0; i < movePosList.Count - 1; i++)
        {
            Debug.DrawLine(movePosList[i], movePosList[i + 1], Color.red, -1);
        }
    }
}
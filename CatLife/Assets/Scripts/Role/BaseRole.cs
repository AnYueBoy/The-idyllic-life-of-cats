using UnityEngine;

public abstract class BaseRole : MonoBehaviour
{
    public abstract void Init();
    public abstract void LocalUpdate(float dt);
}
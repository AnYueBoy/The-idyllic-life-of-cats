using System.Collections.Generic;
using UnityEngine;

public class RoleManager : IManager
{
    private List<BaseRole> roleList;

    private BaseRole mainRole;

    public void Init()
    {
        roleList = new List<BaseRole>();
    }

    public void LocalUpdate(float dt)
    {
        RefreshRoleList(dt);
    }

    private void RefreshRoleList(float dt)
    {
        if (roleList == null || roleList.Count <= 0)
        {
            return;
        }

        foreach (var role in roleList)
        {
            role.LocalUpdate(dt);
        }
    }

    public void AddRole(BaseRole role)
    {
        // TODO: temp logic
        mainRole = role;
        roleList.Add(role);
    }

    public BaseRole MainRole => mainRole;
}
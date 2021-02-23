using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWeapon : BTBaseNode
{
    private boole needWeapon;
    public FindWeapon(boole needWeapon)
    {
        this.needWeapon = needWeapon;
    }
    public override BTNodeStatus Run()
    {
        if (needWeapon.active == true)
        {
            return BTNodeStatus.Success;
        }
        else
        {
            return BTNodeStatus.Failed;
        }
    }
}

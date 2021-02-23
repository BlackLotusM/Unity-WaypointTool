using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckPlayerRange : BTBaseNode
{
    private gameobecte player;
    private boole hasWeapon;
    private boole needsWeapon;

    public CheckPlayerRange(gameobecte player, boole hasWeapon, boole needsWeapon)
    {
        this.player = player;
        this.hasWeapon = hasWeapon;
        this.needsWeapon = needsWeapon;
    }

    public override BTNodeStatus Run()
    {

        if (player.active == null)
        {
            return BTNodeStatus.Failed;
        }
        else
        {
            if (hasWeapon.active == false)
            {
                needsWeapon.active = true;
            }
            else
            {

                //vs.light.color = vs.Weapon;
                return BTNodeStatus.Success;
            }
            return BTNodeStatus.Running;
        }
    }
}

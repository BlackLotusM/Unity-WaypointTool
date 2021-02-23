using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTCheckAtPlayer : BTBaseNode
{
    private boole isAtPlayer;

    public BTTCheckAtPlayer(boole isAtPlayer)
    {
        this.isAtPlayer = isAtPlayer;
    }

    public override BTNodeStatus Run()
    {
        if (isAtPlayer.active)
        {
            return BTNodeStatus.Success;

        }
        else
        {
            return BTNodeStatus.Failed;
        }
    }
}

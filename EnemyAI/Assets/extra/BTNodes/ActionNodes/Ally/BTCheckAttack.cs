using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTCheckAttack : BTBaseNode
{
    private boole underAttack;
    private boole isAtPlayer;
    private NavMeshAgent agent;
    public BTCheckAttack(boole underAttack, boole isAtPlayer, NavMeshAgent agent)
    {
        this.underAttack = underAttack;
        this.isAtPlayer = isAtPlayer;
        this.agent = agent;
    }

    public override BTNodeStatus Run()
    {
        if (underAttack.active)
        {
            agent.enabled = true;
            isAtPlayer.active = false;
            return BTNodeStatus.Success;
        }
        else
        {
            return BTNodeStatus.Failed;
        }    
    }
}

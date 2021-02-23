using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTCheckAttackDone : BTBaseNode
{
    private boole hasAttacked;
    private NavMeshAgent agent;
    public BTCheckAttackDone(boole hasAttacked, NavMeshAgent agent)
    {
        this.hasAttacked = hasAttacked;
        this.agent = agent;
    }

    public override BTNodeStatus Run()
    {
        if (hasAttacked.active)
        {
            agent.enabled = true;
            return BTNodeStatus.Success;

        }
        else
        {
            return BTNodeStatus.Failed;

        }
    }
}
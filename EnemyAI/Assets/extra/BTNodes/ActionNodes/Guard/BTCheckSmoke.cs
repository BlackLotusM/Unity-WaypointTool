using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTCheckSmoke : BTBaseNode
{
    private boole isInSmoke;
    private NavMeshAgent agent;
    public BTCheckSmoke(boole isInSmoke, NavMeshAgent agent)
    {
        this.agent = agent;
        this.isInSmoke = isInSmoke;
    }

    public override BTNodeStatus Run()
    { 
        if (isInSmoke.active)
        {
            agent.isStopped = true;
            return BTNodeStatus.Success;
        }
        else
        {
            agent.isStopped = false;
            return BTNodeStatus.Failed;
        }
    }
}

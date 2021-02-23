using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTMoveAlly : BTBaseNode
{

    private boole hasAttacked;
    private boole isAtPlayer;
    private GameObject child;
    private NavMeshAgent agent;
    private GameObject player;

    public BTMoveAlly(boole hasAttacked, boole isAtPlayer, GameObject child, NavMeshAgent agent, GameObject player)
    {
        this.hasAttacked = hasAttacked;
        this.isAtPlayer = isAtPlayer;
        this.child = child;
        this.agent = agent;
        this.player = player;
    }

    public override BTNodeStatus Run()
    {
        if (hasAttacked.active)
        {
            child.transform.localEulerAngles = new Vector3(0, 180, 0);
            if (player != null)
            {
                agent.SetDestination(player.transform.position);
                agent.gameObject.transform.LookAt(player.transform.position);
            }
        }
        if (player != null)
        {
            if (Vector3.Distance(agent.transform.position, player.transform.position) < 2)
            {
                hasAttacked.active = false;
                isAtPlayer.active = true;
                return BTNodeStatus.Success;
            }
            else
            {
                return BTNodeStatus.Running;
            }
        }
        else
        {
            return BTNodeStatus.Failed;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTFlyArroundPlayer : BTBaseNode
{
    private boole isAtPlayer;
    private NavMeshAgent agent;
    private GameObject child;
    private GameObject player;

    public BTFlyArroundPlayer(boole isAtPlayer, NavMeshAgent agent, GameObject child, GameObject player)
    {
        this.isAtPlayer = isAtPlayer;
        this.agent = agent;
        this.child = child;
        this.player = player;
    }

    public override BTNodeStatus Run()
    {
        if (isAtPlayer.active)
        {
            agent.GetComponent<NavMeshAgent>().enabled = false;
            rotateAroundPlayer();
            return BTNodeStatus.Success;

        }
        else
        {
            return BTNodeStatus.Failed;

        }
    }

    private float angle;
    private float CircleRadius = 4;
    public void rotateAroundPlayer()
    {
        child.transform.localEulerAngles = new Vector3(0, 270, 0);
        Vector3 position = player != null ? player.transform.position : Vector3.zero;
        Vector3 positionOffset = ComputePositionOffset(angle);
        agent.transform.position = position + positionOffset;
        agent.transform.rotation = Quaternion.LookRotation(position - agent.transform.position, player == null ? Vector3.up : player.transform.up);

        angle += Time.deltaTime * 180;
    }
    private Vector3 ComputePositionOffset(float a)
    {
        a *= Mathf.Deg2Rad;

        // Compute the position of the object
        Vector3 positionOffset = new Vector3(
            Mathf.Cos(a) * CircleRadius,
            0,
            Mathf.Sin(a) * CircleRadius
        );
        return positionOffset;
    }
}

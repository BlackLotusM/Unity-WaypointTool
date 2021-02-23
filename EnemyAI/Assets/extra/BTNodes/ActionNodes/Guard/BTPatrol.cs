using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTPatrol : BTBaseNode
{
    private GameObject[] waypoints;
    private int numberOfWayPoints;
    private GameObject target;
    private NavMeshAgent agent;
    private Light light;
    private Color lightColor;

    public BTPatrol(GameObject[] wp, NavMeshAgent agent, Light light, Color lightCol)
    {
        this.light = light;
        this.lightColor = lightCol;
        this.agent = agent;
        this.waypoints = wp;
    }

    public override BTNodeStatus Run()
    {
        agent.SetDestination(waypoints[numberOfWayPoints].gameObject.transform.position);
        light.color = lightColor;
        if (!agent.pathPending && agent.remainingDistance < 1.2f)
        {
            EnemyTowardNextPos();
        }

        return BTNodeStatus.Running;
    }
    void EnemyTowardNextPos()
    {
        if (numberOfWayPoints == waypoints.Length - 1)
        {
            numberOfWayPoints = 0;
            target = waypoints[numberOfWayPoints];
            agent.SetDestination(target.transform.position);
        }
        else
        {
            numberOfWayPoints++;
            target = waypoints[numberOfWayPoints];
            agent.SetDestination(target.transform.position);
        }
    }
}

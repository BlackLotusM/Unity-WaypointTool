using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTFlyToCover : BTBaseNode
{
    private NavMeshAgent agent;
    private GameObject[] cover;
    private GameObject coverToDo;
    public BTFlyToCover(NavMeshAgent agent, GameObject[] cover)
    {
        this.agent = agent;
        this.cover = cover;
    }

    public override BTNodeStatus Run()
    {
        if(coverToDo == null)
        {
            FindTarget();
        }
        else
        {
            Vector3 coverPos = coverToDo.transform.position + (coverToDo.transform.position - GameObject.FindGameObjectWithTag("enemy").transform.position).normalized * 4;
            agent.transform.LookAt(coverToDo.transform.position);
            
            agent.SetDestination(coverPos);
        }
        
        if(Vector3.Distance(agent.transform.position, coverToDo.transform.position) < 5)
        {
            coverToDo = null;
            return BTNodeStatus.Success;
        }
            return BTNodeStatus.Running;
    }

    void FindTarget()
    {
        float lowestDist = Mathf.Infinity;

        for (int i = 0; i < cover.Length; i++)
        {

            float dist = Vector3.Distance(cover[i].transform.position, agent.transform.position);

            if (dist < lowestDist)
            {
                lowestDist = dist;
                coverToDo = cover[i];
            }
        }
    }
}

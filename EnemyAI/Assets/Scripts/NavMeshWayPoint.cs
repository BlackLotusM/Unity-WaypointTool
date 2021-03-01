using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshWayPoint : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    public ArraySO test4;
    public List<GameObject> waypoints;
    private GameObject target;
    private int numberOfWayPoints;
    


    //private void Update()
    //{
    //    waypoints = test4.itemList.;
    //    agent.SetDestination(waypoints[numberOfWayPoints].gameObject.transform.position);
    //    if (!agent.pathPending && agent.remainingDistance < 1.2f)
    //    {
    //        EnemyTowardNextPos();
    //    }
    //}

    //void EnemyTowardNextPos()
    //{
    //    if (numberOfWayPoints == waypoints.Count - 1)
    //    {
    //        numberOfWayPoints = 0;
    //        target = waypoints[numberOfWayPoints];
    //        agent.SetDestination(target.transform.position);
    //    }
    //    else
    //    {
    //        numberOfWayPoints++;
    //        target = waypoints[numberOfWayPoints];
    //        agent.SetDestination(target.transform.position);
    //    }
    //}
}

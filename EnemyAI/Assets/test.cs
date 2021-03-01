using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    public List<Vector3> plaatsen;
    public ArraySO list;
    private void Start()
    {
        for(int i = 0; i < list.WaypointList.Count; i++)
        {
            plaatsen.Add(list.WaypointList[i].Coords);
        }
    }
}

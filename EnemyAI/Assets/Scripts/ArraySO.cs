using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ArraySO_", menuName = "Scrip/ArraySO")]
[Serializable]
public class ArraySO : ScriptableObject
{
    [Serializable]
    public class Waypoint
    {
        public string name = "Waypoint";
        public Vector3 Coords;
    }

    public List<Waypoint> WaypointList = new List<Waypoint>();
}
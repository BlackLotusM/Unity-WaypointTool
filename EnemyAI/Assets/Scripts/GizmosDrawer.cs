using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GizmosDrawer : MonoBehaviour
{
    public ArraySO ar;
    public ArraySO oldAr;

    public GameObject gizmoPref;

    public List<GameObject> Spawned;

    void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            SceneView.RepaintAll();
        }
    }
    public void sel(int i)
    {
        Selection.activeGameObject = Spawned[i];
    }
    public void OnDrawGizmos()
    {
        if (oldAr != ar)
        {
            //Checks For array change
            Debug.Log("Change");
            deleteObjectList();
        }

        for (int i = 0; i < Spawned.Count; i++)
        {
            if (Spawned[i] == null)
            {
                deleteObjectList();
            }
        }

        if(Spawned.Count > ar.WaypointList.Count)
        {
            deleteObjectList();
        }

        for (int i = Spawned.Count; i < ar.WaypointList.Count; i++)
        {
            GameObject test = Instantiate(gizmoPref, ar.WaypointList[i].Coords, Quaternion.identity);
            test.name = ar.WaypointList[i].name;
            Spawned.Add(test);
        }


        for (var i = 0; i < ar.WaypointList.Count; i++)
        {
            if (Spawned[i].name != ar.WaypointList[i].name)
            {
                deleteObjectList();
                return;
            }
        }

        for (var i = 1; i < ar.WaypointList.Count; i++)
        {
            Debug.DrawLine(ar.WaypointList[i - 1].Coords, ar.WaypointList[i].Coords, Color.red);
        }

        for (int i = 0; i < Spawned.Count; i++)
        {
            if (ar.WaypointList[i] == null)
            {

            }
            else
            {
                ar.WaypointList[i].Coords = Spawned[i].transform.position;
            }
        }

        

        if(ar.WaypointList.Count != Spawned.Count)
        {
            deleteObjectList();
        }

        EditorUtility.SetDirty(ar);
        oldAr = ar;
    }

    private void deleteObjectList()
    {
        foreach (GameObject go in Spawned)
        {
            DestroyImmediate(go);
        }
        Spawned.Clear();
    }
}

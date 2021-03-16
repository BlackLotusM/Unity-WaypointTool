using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GizmosDrawer : MonoBehaviour
{
    public bool loop, navmesh;
    public ArraySO loadedArray;
    public GameObject parent;
    public GameObject aiBoyPrefab;
    public List<GameObject> Spawned = new List<GameObject>();
    public GameObject gizmoPref;

    private GameObject aiBoy;
    private ArraySO oldAr;
    private NavMeshPath path, pathLoop;

    public void sel(int i)
    {
        Selection.activeGameObject = Spawned[i];
    }

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {

        if (GameObject.FindObjectOfType<GizmosDrawer>() != null)
        {
            GizmosDrawer myComponent = GameObject.FindObjectOfType<GizmosDrawer>().GetComponent<GizmosDrawer>();
            if (myComponent == GameObject.FindObjectOfType<GizmosDrawer>().GetComponent<GizmosDrawer>())
            {
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(myComponent, true);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (path == null)
        {
            path = new NavMeshPath();
        }

        if (loadedArray == null)
        {
            foreach (GameObject go in Spawned)
            {
                try
                {
                    DestroyImmediate(go);
                }
                catch { }
            }
            DestroyImmediate(this.gameObject);
        }

        if (loadedArray == null) { return; }

        if (oldAr != loadedArray) { deleteObjectList(); }

        for (int i = 0; i < Spawned.Count; i++)
        {
            if (Spawned[i] == null)
            {
                deleteObjectList();
            }
        }

        if (parent == null)
        {
            GameObject t = new GameObject("Waypoints");
            parent = t;
            t.transform.SetParent(this.transform);
        }

        if (aiBoy == null && !GameObject.FindObjectOfType<Patrol>().gameObject)
        {
            GameObject t = Instantiate(aiBoyPrefab);
            t.transform.SetParent(this.gameObject.transform);
            aiBoy = t;

        }
        else
        {
            if (aiBoy == null)
            {
                aiBoy = GameObject.FindObjectOfType<Patrol>().gameObject;
            }
            aiBoy.GetComponent<Patrol>().points = loadedArray;
        }

        if (Spawned.Count > loadedArray.WaypointList.Count) { deleteObjectList(); }

        for (int i = Spawned.Count; i < loadedArray.WaypointList.Count; i++)
        {
            GameObject test = Instantiate(gizmoPref, loadedArray.WaypointList[i].Coords, Quaternion.identity);
            test.name = loadedArray.WaypointList[i].name;
            test.transform.SetParent(parent.transform);

            Spawned.Add(test);
        }

        for (var i = 0; i < loadedArray.WaypointList.Count; i++)
        {
            if (Spawned[i].name != loadedArray.WaypointList[i].name)
            {
                deleteObjectList();
                return;
            }
        }

        for (var i = 1; i < loadedArray.WaypointList.Count; i++)
        {

            if (navmesh)
            {
                if (path == null)
                {
                    path = new NavMeshPath();
                }

                NavMesh.CalculatePath(Spawned[i - 1].transform.position, loadedArray.WaypointList[i].Coords, NavMesh.AllAreas, path);
                for (int t = 0; t < path.corners.Length - 1; t++)
                    Debug.DrawLine(path.corners[t], path.corners[t + 1], Color.green);

                if (loop)
                {
                    if (pathLoop == null)
                    {
                        pathLoop = new NavMeshPath();
                    }

                    NavMesh.CalculatePath(Spawned[0].transform.position, loadedArray.WaypointList[loadedArray.WaypointList.Count - 1].Coords, NavMesh.AllAreas, pathLoop);
                    for (int t = 0; t < pathLoop.corners.Length - 1; t++)
                        Debug.DrawLine(pathLoop.corners[t], pathLoop.corners[t + 1], Color.green);
                }

            }
            else
            {
                Debug.DrawLine(loadedArray.WaypointList[i - 1].Coords, loadedArray.WaypointList[i].Coords, Color.red);
                if (loop)
                {
                    Debug.DrawLine(loadedArray.WaypointList[0].Coords, loadedArray.WaypointList[loadedArray.WaypointList.Count - 1].Coords, Color.red);
                }
            }
        }

        if (loadedArray.WaypointList.Count != Spawned.Count)
        {
            deleteObjectList();
        }

        for (int i = 0; i < Spawned.Count; i++)
        {
            if (loadedArray.WaypointList[i] != null)
            {
                loadedArray.WaypointList[i].Coords = Spawned[i].transform.position;
            }
        }

        EditorUtility.SetDirty(loadedArray);
        oldAr = loadedArray;
        SceneView.RepaintAll();
    }

    public void Redo()
    {
        deleteObjectList();
        try
        {
            for (int i2 = Spawned.Count; i2 < loadedArray.WaypointList.Count; i2++)
            {
                GameObject test = Instantiate(gizmoPref, loadedArray.WaypointList[i2].Coords, Quaternion.identity);
                test.transform.SetParent(parent.transform);
                test.name = loadedArray.WaypointList[i2].name;
                Spawned.Add(test);
            }
        }
        catch
        {

        }
    }

    private void deleteObjectList()
    {
        if (Spawned.Count != 0)
        {
            foreach (GameObject go in Spawned)
            {
                DestroyImmediate(go);
            }
        }
        Spawned.Clear();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class GizmosDrawer : MonoBehaviour
{
    public ArraySO loadedArray;
    private ArraySO oldAr;
    public GameObject gizmoPref;
    public bool loop;
    public bool follow;

    public List<GameObject> Spawned = new List<GameObject>();

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

    public void Redo()
    {
        deleteObjectList();
        for (int i2 = Spawned.Count; i2 < loadedArray.WaypointList.Count; i2++)
        {
            GameObject test = Instantiate(gizmoPref, loadedArray.WaypointList[i2].Coords, Quaternion.identity);
            test.name = loadedArray.WaypointList[i2].name;
            Spawned.Add(test);
        }
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
        if(loadedArray == null)
        {
            foreach (GameObject go in Spawned)
            {
                try
                {
                    DestroyImmediate(go);
                }
                catch { }
            }
        }
        foreach(GameObject go in Spawned)
        {
            //go.GetComponent<DrawSphereGizmo>().followMouse = follow;
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

        

        if (Spawned.Count > loadedArray.WaypointList.Count) { deleteObjectList(); }

        for (int i = Spawned.Count; i < loadedArray.WaypointList.Count; i++)
        {
            GameObject test = Instantiate(gizmoPref, loadedArray.WaypointList[i].Coords, Quaternion.identity);
            test.name = loadedArray.WaypointList[i].name;
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
            Debug.DrawLine(loadedArray.WaypointList[i - 1].Coords, loadedArray.WaypointList[i].Coords, Color.red);
            if (loop)
            {
                Debug.DrawLine(loadedArray.WaypointList[0].Coords, loadedArray.WaypointList[loadedArray.WaypointList.Count - 1].Coords, Color.red);
            }
        }

        

        if(loadedArray.WaypointList.Count != Spawned.Count)
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

    private void deleteObjectList()
    {
        if(Spawned.Count != 0) { 
            foreach (GameObject go in Spawned)
            {
                DestroyImmediate(go);
            }
        }
        Spawned.Clear();
    }
}

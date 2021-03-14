using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DrawSphereGizmo : MonoBehaviour
{
    public bool followMouse;

    Ray mousepos;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, 0.8f);
        mousepos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    }

    private void Update()
    {
        if (Selection.activeObject == this.gameObject)
        {
            SceneView sceneView = (SceneView)SceneView.sceneViews[0];
            sceneView.Focus();
            if (followMouse)
            {
                RaycastHit hit;
                Ray pRay = mousepos;
                if (Physics.Raycast(pRay, out hit))
                {
                    if (hit.collider)
                    {
                        this.transform.position = hit.point;
                    }
                }
            }
            else
            {

            }
        }
    }
}

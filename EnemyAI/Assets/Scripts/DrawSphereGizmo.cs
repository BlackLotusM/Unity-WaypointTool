using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSphereGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, 0.8f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getCam : MonoBehaviour
{
    public Camera cam;
    private void OnDrawGizmos()
    {
        cam = Camera.main;
    }
}

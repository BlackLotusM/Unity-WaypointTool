// ShowGoldenPath
using UnityEngine;
using UnityEngine.AI;

public class ShowGoldenPath : MonoBehaviour
{
    public Transform target;
    public NavMeshPath path;
    public float elapsed = 0.0f;
    void Start()
    {
        //path = new NavMeshPath();
        elapsed = 0.0f;
    }

    private void OnDrawGizmos()
    {
        if(path == null)
        {
            path = new NavMeshPath();
        }
        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }

    void Update()
    {
        
    }
}
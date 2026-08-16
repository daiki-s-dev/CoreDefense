using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    // 子オブジェクトとして配置したWaypoint
    public Transform[] waypoints;

    private void Awake()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            waypoints = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                waypoints[i] = transform.GetChild(i);
            }
        }
    }

    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length)
            return null;

        return waypoints[index];
    }

    public int WaypointCount => waypoints.Length;
}
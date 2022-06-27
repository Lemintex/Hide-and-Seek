using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    public float moveSpeed = 4;
    public float rotateSpeed = 90   ;
    public float waitTime = .1f;
    public Transform path;
    Vector3[] waypoints;

    public Light spotlight;
    public float viewDistance;
    float viewAngle;

    Transform playerTransform;

    public LayerMask viewMask;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        waypoints = new Vector3[path.childCount];
        for(int i = 0; i < path.childCount; i++)
        {
            waypoints[i] = new Vector3(path.GetChild(i).position.x, transform.position.y, path.GetChild(i).position.z);
        }
        StartCoroutine(FollowPath());
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            spotlight.color = Color.blue;
        }
        else
        {
            spotlight.color = Color.yellow;
        }
    }

    bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) < viewDistance)
        {
            return false;
        }

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if(angleBetweenGuardAndPlayer > viewAngle / 2)
        {
            return false;
        }
        Ray ray = new Ray(transform.position, playerTransform.position);

        RaycastHit hitInfo;
        if(Physics.Linecast(transform.position, playerTransform.position, viewMask))
        {
            return false;
        }
        return true;
    }
    IEnumerator FollowPath()
    {
        transform.position = waypoints[0];
        int targetIndex = 1;
        Vector3 targetPosition = waypoints[targetIndex];
        transform.LookAt(targetPosition);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if(transform.position == targetPosition)
            {
                targetIndex = (targetIndex + 1) % waypoints.Length;
                targetPosition = waypoints[targetIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToNextWaypoint(targetPosition));
            }
            yield return null;
        }
    }

    IEnumerator TurnToNextWaypoint(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotateSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;// ew WaitForEndOfFrame();
        }
    }
     
    private void OnDrawGizmos()
    {
        Vector3 startPos = path.GetChild(0).position;
        Vector3 previousPos = startPos;
        foreach(Transform waypoint in path)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }
        Gizmos.DrawLine(previousPos, startPos);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Transform frontPoint;
    public Transform backPoint;

    Vector3 upDirection;
    Vector3 forwardDirection;

    private void Start()
    {
        
    }

    private void Update()
    {
        Vector3 backWorld = transform.TransformPoint(backPoint.position);
        Vector3 frontWorld = transform.TransformPoint(frontPoint.position);

        upDirection = Vector3.Cross(backWorld, frontWorld).normalized;
        forwardDirection = Vector3.Cross(upDirection, transform.right);
        //transform.LookAt(transform.position + forwardDirection);

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + upDirection * 25);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + forwardDirection * 25);
    }
}

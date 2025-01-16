using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float forwardForce = 10.0f;
    public float steeringTorque = 10.0f;

    public float desiredHeight = 1.0f;
    public float spring = 100.0f;
    public float damp = 10.0f;

    Rigidbody rb;

    public int numPointsPerAxis = 2;
    public float contactRadius = 0.25f;

    List<GameObject> points;

    LayerMask mask;

    Vector3 upDirection;
    Vector3 forwardDirection;

    private void Start()
    {
        points = new List<GameObject>();
        //In case of 10 by 5 create points between -5 - 5 and -2.5 - 2.5
        Vector3 origin = -transform.localScale / 2f;

        for (int x = 0; x < numPointsPerAxis; x++)
        {
            for (int z = 0; z < numPointsPerAxis; z++)
            {
                Vector3 pointPosition = transform.position + origin + new Vector3(x * (transform.localScale.x/(numPointsPerAxis - 1)), 0, z * (transform.localScale.z/(numPointsPerAxis - 1)));
                GameObject point = new GameObject("Point");
                point.transform.parent = transform;
                point.transform.position = pointPosition;
                points.Add(point);
            }
        }

        rb = GetComponent<Rigidbody>();

        mask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * forwardForce);
        }

        float steer = Input.GetAxis("Horizontal") * steeringTorque;
        rb.AddTorque(steer * transform.up);

        for(int i = 0; i < points.Count; i++)
        {
            if (Physics.Raycast(points[i].transform.position, -transform.up, out RaycastHit hitInfo, 5.0f, mask))
            {
                Vector3 cancelGravity = rb.mass * Physics.gravity * 0.2f;

                float yDifference = (hitInfo.point.y + desiredHeight) - points[i].transform.position.y;
                float force = yDifference * spring;
                force -= rb.GetPointVelocity(points[i].transform.position).y * damp;

                rb.AddForceAtPosition(cancelGravity + force * transform.up, points[i].transform.position);
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (points == null) return;

        Gizmos.color = Color.white;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i].transform.position, contactRadius);
        }
    }
}

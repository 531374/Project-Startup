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
    [SerializeField] private float pickupRange;

    [SerializeField] private Transform leftThing;
    [SerializeField] private Transform rightThing;

    Rigidbody rb;

    public int numPointsPerAxis = 2;
    public float contactRadius = 0.25f;

    List<GameObject> points;

    LayerMask mask;

    Vector3 upDirection;
    Vector3 forwardDirection;

    public Transform camTransform;
    public Vector3 camOffset;
    public float camSpeed = 1.0f;
    public float camRotationSpeed = 1.0f;

    bool moveForward = false;
    float steer = 0.0f;

    private void Start()
    {
        points = new List<GameObject>();
        //Divide points over the ship's (hitbox) surface
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
        camTransform.position = transform.position + camOffset;


    }

    private void Update()
    {
        GetInput();
        Interact ();
        //MoveCamera();        

        DeformTerrain ();

    }

    private void FixedUpdate()
    {
        if (moveForward)
        {
            rb.AddForce(transform.forward * forwardForce);
        }

        rb.AddTorque(steer * steeringTorque * transform.up);

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

    private void LateUpdate()
    {
        MoveCamera();
    }

    void GetInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            moveForward = true;
        }
        else
        {
            moveForward = false;
        }

        steer = Input.GetAxis("Horizontal");
    }

    void MoveCamera()
    {
        Vector3 camPos = camTransform.position;
        Vector3 desiredPos = transform.position + transform.right * camOffset.x + transform.up * camOffset.y + transform.forward * camOffset.z;
        camTransform.position = Vector3.Lerp(camPos, desiredPos, camSpeed * Time.deltaTime);

        Quaternion camRotation = camTransform.rotation;
        Vector3 lookDirection = rb.velocity.magnitude > 5f ? rb.velocity.normalized : transform.forward;
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
        camTransform.rotation = Quaternion.Slerp(camRotation, desiredRotation, camRotationSpeed * Time.deltaTime);
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

    private void Interact ()
    {
        if (Input.GetKeyDown (KeyCode.E))
        {
            Collider[] colliders = Physics.OverlapSphere (this.transform.position, pickupRange);
            
            if (colliders.Length < 0) return;

            foreach (var collider in colliders)
            {
                Interactable interactable = collider.gameObject.GetComponent <Interactable> ();


                if (interactable != null && Vector3.Distance (this.transform.position, interactable.transform.position) < interactable.radius)
                {
                    interactable.interacted = true;
                }
            } 
            
        }
    }

    void DeformTerrain()
    {
        if(rb.velocity.magnitude >= 2.5f)
        {
            if (Physics.Raycast(leftThing.position, -transform.up, out RaycastHit hitLeft))
            {
                if (hitLeft.transform.TryGetComponent<TerrainEditor>(out TerrainEditor terrain)) terrain.DeformTerrainAtPoint(hitLeft.point);
            }

            if (Physics.Raycast(rightThing.position, -transform.up, out RaycastHit hitRight))
            {
                if (hitRight.transform.TryGetComponent<TerrainEditor>(out TerrainEditor terrain1)) terrain1.DeformTerrainAtPoint(hitRight.point);
            }
        }
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position + new Vector3 (0, this.GetComponent<Collider> ().bounds.size.y / 2, 0), pickupRange);
    }
}

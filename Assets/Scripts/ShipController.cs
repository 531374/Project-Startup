using UnityEngine;

public class ShipController : MonoBehaviour
{
    private Rigidbody rb;
    public float moveForce = 50f;
    public float rotationSpeed = 10f;
    public float raycastDistance = 10f;
    public float targetHeight = 2f;
    public float heightAdjustSpeed = 5f;

    // Corner points for terrain detection
    public Transform frontLeftPoint;
    public Transform frontRightPoint;
    public Transform backLeftPoint;
    public Transform backRightPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Create corner points if they don't exist
        if (frontLeftPoint == null)
            frontLeftPoint = CreatePoint("FrontLeftPoint", new Vector3(-0.5f, 0, 1f));
        
        if (frontRightPoint == null)
            frontRightPoint = CreatePoint("FrontRightPoint", new Vector3(0.5f, 0, 1f));
        
        if (backLeftPoint == null)
            backLeftPoint = CreatePoint("BackLeftPoint", new Vector3(-0.5f, 0, -1f));
        
        if (backRightPoint == null)
            backRightPoint = CreatePoint("BackRightPoint", new Vector3(0.5f, 0, -1f));
    }

    private Transform CreatePoint(string name, Vector3 localPosition)
    {
        GameObject point = new GameObject(name);
        Transform pointTransform = point.transform;
        pointTransform.SetParent(transform);
        pointTransform.localPosition = localPosition;
        return pointTransform;
    }

    void FixedUpdate()
    {
        AdjustToTerrain();
        MoveShip();
    }

    private void AdjustToTerrain()
    {
        AlignCornerToTerrain(frontLeftPoint);
        AlignCornerToTerrain(frontRightPoint);
        AlignCornerToTerrain(backLeftPoint);
        AlignCornerToTerrain(backRightPoint);
    }

    private void AlignCornerToTerrain(Transform cornerPoint)
    {
        RaycastHit hit;
        if (Physics.Raycast(cornerPoint.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Vector3 hitNormal = hit.normal;

                // Align corner's local up vector to the terrain's normal
                Quaternion targetRotation = Quaternion.FromToRotation(cornerPoint.up, hitNormal) * cornerPoint.rotation;
                
                // Smooth rotation
                cornerPoint.rotation = Quaternion.Slerp(cornerPoint.rotation, targetRotation, Time.fixedDeltaTime * heightAdjustSpeed);
            }
        }
    }

    private void MoveShip()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
        }

        if (moveDirection != Vector3.zero)
        {
            rb.AddForce(moveDirection.normalized * moveForce, ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -rotationSpeed * Time.fixedDeltaTime, 0));
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationSpeed * Time.fixedDeltaTime, 0));
        }
    }

    void OnDrawGizmos()
    {
        if (frontLeftPoint != null && frontRightPoint != null && 
            backLeftPoint != null && backRightPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(frontLeftPoint.position, frontLeftPoint.position + Vector3.down * raycastDistance);
            Gizmos.DrawLine(frontRightPoint.position, frontRightPoint.position + Vector3.down * raycastDistance);
            Gizmos.DrawLine(backLeftPoint.position, backLeftPoint.position + Vector3.down * raycastDistance);
            Gizmos.DrawLine(backRightPoint.position, backRightPoint.position + Vector3.down * raycastDistance);
        }
    }
}

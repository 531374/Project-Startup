<<<<<<< Updated upstream
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
=======
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipBuoyancy : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform frontFloater; // Front position of the ship
    [SerializeField] private Transform backFloater;  // Back position of the ship
    [SerializeField] private Transform[] floaters;   // Positions of floaters for buoyancy
    [SerializeField] private float sandDrag = 0.99f;
    [SerializeField] private float sandAngularDrag = 0.5f;
    [SerializeField] private float depthBeforeSubmerge = 1f;
    [SerializeField] private LayerMask terrainMask;  // Layer mask for the terrain
    [SerializeField] private float moveForce = 10f;  // Force applied for movement
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation adjustment
>>>>>>> Stashed changes

    void Start()
    {
        rb = GetComponent<Rigidbody>();
<<<<<<< Updated upstream
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
=======
>>>>>>> Stashed changes
    }

    void FixedUpdate()
    {
<<<<<<< Updated upstream
=======
        ApplyBuoyancy();
>>>>>>> Stashed changes
        AdjustToTerrain();
        MoveShip();
    }

<<<<<<< Updated upstream
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
=======
    private void ApplyBuoyancy()
    {
        foreach (Transform floater in floaters)
        {
            Vector3 floaterPosition = floater.position;
            float waveHeight = GetTerrainHeight(floaterPosition);

            if (floaterPosition.y < waveHeight)
            {
                float displacementMultiplier = Mathf.Clamp01((waveHeight - floaterPosition.y) / depthBeforeSubmerge);
                Vector3 buoyancyForce = new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f);
                rb.AddForceAtPosition(buoyancyForce, floaterPosition, ForceMode.Acceleration);

                rb.AddForce(displacementMultiplier * -rb.velocity * sandDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rb.AddForce(displacementMultiplier * -rb.angularVelocity * sandAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }

    private void AdjustToTerrain()
    {
        // Get terrain height and normal at the front and back positions
        Vector3 frontPosition = frontFloater.position;
        Vector3 backPosition = backFloater.position;

        if (Physics.Raycast(frontPosition + Vector3.up * 10f, Vector3.down, out RaycastHit frontHit, 20f, terrainMask) &&
            Physics.Raycast(backPosition + Vector3.up * 10f, Vector3.down, out RaycastHit backHit, 20f, terrainMask))
        {
            // Calculate the new forward direction based on terrain slope
            Vector3 newForward = (frontHit.point - backHit.point).normalized;
            Vector3 newUp = Vector3.Cross(newForward, transform.right).normalized;

            // Smoothly rotate the ship to align with the terrain
            Quaternion targetRotation = Quaternion.LookRotation(newForward, newUp);
            rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
>>>>>>> Stashed changes
        }
    }

    private void MoveShip()
    {
<<<<<<< Updated upstream
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
=======
        // Move forward when "W" is pressed
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * moveForce, ForceMode.Force);
        }

        // Move backward when "S" is pressed
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * moveForce, ForceMode.Force);
        }
    }

    // Get terrain height at a specific position
    private float GetTerrainHeight(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, terrainMask))
        {
            return hit.point.y;
        }
        return 0f;
>>>>>>> Stashed changes
    }

    void OnDrawGizmos()
    {
<<<<<<< Updated upstream
        if (frontLeftPoint != null && frontRightPoint != null && 
            backLeftPoint != null && backRightPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(frontLeftPoint.position, frontLeftPoint.position + Vector3.down * raycastDistance);
            Gizmos.DrawLine(frontRightPoint.position, frontRightPoint.position + Vector3.down * raycastDistance);
            Gizmos.DrawLine(backLeftPoint.position, backLeftPoint.position + Vector3.down * raycastDistance);
            Gizmos.DrawLine(backRightPoint.position, backRightPoint.position + Vector3.down * raycastDistance);
=======
        if (floaters != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform floater in floaters)
            {
                Gizmos.DrawSphere(floater.position, 0.1f);
            }
        }

        // Visualize front and back points
        if (frontFloater != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(frontFloater.position, 0.1f);
        }

        if (backFloater != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(backFloater.position, 0.1f);
>>>>>>> Stashed changes
        }
    }
}

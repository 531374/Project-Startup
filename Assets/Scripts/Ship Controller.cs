using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMODUnity;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject keyCapPrefab;

    private Rigidbody rb;

    public int numPointsPerAxis = 2;
    public float contactRadius = 0.25f;

    private List<GameObject> points;

    private LayerMask mask;

    private Vector3 upDirection;
    private Vector3 forwardDirection;

    public Transform camTransform;
    public Vector3 camOffset;
    public float camSpeed = 1.0f;
    public float camRotationSpeed = 1.0f;

    private bool moveForward = false;
    private bool moveBackward = false;
    private float steer = 0.0f;

    public bool isEnabled;

    private GameObject keyCap;

    // Reference to the FMOD Event Emitter on the child GameObject
    [SerializeField] private StudioEventEmitter sandBoatEmitter;
    
    private bool isMoving = false;
    private float movementThreshold = 0.1f;

    private void Start()
    {
        points = new List<GameObject>();
        Vector3 origin = -transform.localScale / 2f;

        for (int x = 0; x < numPointsPerAxis; x++)
        {
            for (int z = 0; z < numPointsPerAxis; z++)
            {
                Vector3 pointPosition = transform.position + origin + new Vector3(x * (transform.localScale.x / (numPointsPerAxis - 1)), 0, z * (transform.localScale.z / (numPointsPerAxis - 1)));
                GameObject point = new GameObject("Point");
                point.transform.parent = transform;
                point.transform.position = pointPosition;
                points.Add(point);
            }
        }

        rb = GetComponent<Rigidbody>();

        mask = LayerMask.GetMask("Ground");
        camTransform.position = transform.position + camOffset;

        isEnabled = true;

        transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), PlayerPrefs.GetFloat("PlayerZ"));
        player.GetComponent<PlayerController>().isEnabled = false;
        player.SetActive(false);
        playerCanvas.SetActive(false);
    }

    private void Update()
    {
        ShowKeyCap();
        if (!isEnabled) return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            player.SetActive(true);
            playerCanvas.SetActive(true);
            this.isEnabled = false;
            player.GetComponent<PlayerController>().isEnabled = true;

            if (Physics.Raycast(transform.position, transform.right, out RaycastHit hit))
            {
                player.transform.position = hit.distance > 15.0f ? transform.position + (transform.right * 15.0f) : hit.point;
            }
            else
            {
                player.transform.position = transform.position + (transform.right * 15.0f);
            }
        }

        GetInput();
        Interact();
        DeformTerrain();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < points.Count; i++)
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

        if (!isEnabled) return;

        bool wasMoving = isMoving;
        isMoving = rb.velocity.magnitude > movementThreshold && moveForward;

        // Handle sound state changes using the Event Emitter
        if (isMoving && !wasMoving)
        {
            sandBoatEmitter.Play();
        }
        else if (!isMoving && wasMoving)
        {
            sandBoatEmitter.Stop();
        }

        if (moveForward)
        {
            rb.AddForce(transform.forward * forwardForce);
        }
        else if (moveBackward)
        {
            rb.AddForce(-transform.forward * forwardForce * 0.25f);
        }

        rb.AddTorque(steer * steeringTorque * transform.up);
    }

    private void LateUpdate()
    {
        if (!isEnabled) return;
        MoveCamera();
    }

    void GetInput()
    {
        moveForward = Input.GetKey(KeyCode.W);
        moveBackward = Input.GetKey(KeyCode.S);

        steer = Input.GetAxis("Horizontal");
    }

    void MoveCamera()
    {
        if (BookManager.instance.isBookOpened) return;

        Vector3 camPos = camTransform.position;
        Vector3 desiredPos = transform.position + transform.right * camOffset.x + transform.up * camOffset.y + transform.forward * camOffset.z;
        camTransform.position = Vector3.Lerp(camPos, desiredPos, camSpeed * Time.deltaTime);

        Quaternion camRotation = camTransform.rotation;
        Vector3 lookDirection = moveForward ? rb.velocity.normalized : transform.forward;
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
        camTransform.rotation = Quaternion.Slerp(camRotation, desiredRotation, camRotationSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        sandBoatEmitter.Stop();
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

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

            if (colliders.Length < 0) return;

            foreach (var collider in colliders)
            {
                Interactable interactable = collider.gameObject.GetComponent<Interactable>();

                if (interactable != null && Vector3.Distance(transform.position, interactable.transform.position) < interactable.radius)
                {
                    interactable.interacted = true;
                }
            }
        }
    }

    void DeformTerrain()
    {
        if (rb.velocity.magnitude >= 2.5f)
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

    private void ShowKeyCap()
    {
        if (isEnabled) return;
        if (keyCap == null && Vector3.Distance(transform.position, player.transform.position) < 15.0f)
        {
            keyCap = Instantiate(keyCapPrefab, transform.position + new Vector3(0, 10.0f, 0), Quaternion.identity, transform);
        }
        else if (keyCap != null && Vector3.Distance(player.transform.position, transform.position) > 150.0f)
        {
            Destroy(keyCap);
        }

        if (keyCap != null) keyCap.transform.LookAt(Camera.main.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, GetComponent<Collider>().bounds.size.y / 2, 0), pickupRange);
    }
}


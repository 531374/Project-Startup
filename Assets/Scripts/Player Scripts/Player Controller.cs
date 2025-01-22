using System.Collections;
using UnityEngine;


[RequireComponent(typeof(PlayerHealthManager))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] Animator anim;

    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float rotationSpeed;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashCooldown = 2.5f;
    [SerializeField] private float dashDuration = 0.75f;
    private bool canDash = true;

    [Header("Camera Settings")]
    [SerializeField] private float dashFovIncrease = 20f; // Amount by which FOV increases during dash
    [SerializeField] private float fovChangeSpeed = 5f;   // Speed of FOV change

    [Header ("Interactable Settings")]
    [SerializeField] public float pickupRange;

    public float damage = 10f;

    private float defaultFov;  // The camera's default FOV

    Rigidbody rb;

    public static PlayerController instance;

    [SerializeField] private Stamina stamina;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EventBus<SwordHitEvent>.OnEvent += TakeHit;

        defaultFov = cam.fieldOfView;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        Move();
        Interact();
        if (stamina.currentStamina <= 10) return;

        if (Input.GetMouseButtonDown(0))
        {
            //anim.SetTrigger("Swing");
            //stamina.ChangeStamina(5);
        }

        // Gradually return FOV to default if not dashing
        if (canDash && Mathf.Abs(cam.fieldOfView - defaultFov) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, Time.deltaTime * fovChangeSpeed);
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
                    Debug.Log ("Interacting");
                }
            } 
            
        }
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position + new Vector3 (0, this.GetComponent<Collider> ().bounds.size.y / 2, 0), pickupRange);
    }

    private IEnumerator dash(Vector3 direction)
    {
        canDash = false;

        // Increase FOV for dash effect
        float targetFov = defaultFov + dashFovIncrease;

        // Apply initial dash force
        rb.velocity = Vector3.zero; // Reset velocity for clean dash
        rb.AddForce(direction * dashingPower, ForceMode.Impulse);

        // Dash movement phase
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            // Smoothly adjust camera FOV during the dash
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovChangeSpeed);
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Stop dash movement (optional damping or reset velocity here if needed)
        rb.velocity = Vector3.zero;

        // Slowing down phase: return FOV to default
        while (Mathf.Abs(cam.fieldOfView - defaultFov) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, Time.deltaTime * fovChangeSpeed);
            yield return null; // Wait for the next frame
        }

        // Ensure FOV is exactly at default (handle rounding errors)
        cam.fieldOfView = defaultFov;

        yield return new WaitForSeconds(dashCooldown); // Wait for cooldown
        canDash = true;
        stamina.ChangeStamina(10); // Reduce stamina after dash
    }



    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        input.Normalize();

        float dx = input.x * speed * Time.deltaTime;
        float dz = input.z * speed * Time.deltaTime;

        transform.Translate(dx, 0, dz);

        Vector3 dashDirection = (input.x * transform.right + input.z * transform.forward).normalized;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(dash(dashDirection));

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(0, mouseX, 0);

        if (Physics.Raycast(cam.transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            if (hitInfo.distance < 0.5f)
            {
                //Camera can move up but not down
                mouseY = Mathf.Max(mouseY, 0.0f);
            }
        }

        // Clamp rotation so you don't go over the player
        float dotProduct = Vector3.Dot(cam.transform.forward, Vector3.down);

        //Camera can move down but not up
        if (dotProduct >= 0.75f) mouseY = Mathf.Min(mouseY, 0.0f);

        // Rotate around the player
        cam.transform.RotateAround(transform.position, transform.right, mouseY);

        anim.SetFloat("Movement", input.magnitude * Mathf.Sign(dz));
    }

    void TakeHit(SwordHitEvent pEvent)
    {
    }

    void Attack()
    {

    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(PlayerHealthManager))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] Animator anim;
    [SerializeField] Stamina stamina;

    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float jumpStrength;
    [SerializeField] private float rotationSpeed;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashCooldown = 2.5f;
    [SerializeField] private float dashDuration = 1f;

    [HideInInspector] public bool isDashing;
    private bool canDash;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float dashFovIncrease = 20f; // Amount by which FOV increases during dash
    [SerializeField] private float fovChangeSpeed = 5f;   // Speed of FOV change

    [Header ("Interactable Settings")]
    [SerializeField] public float pickupRange;

    public float damage = 10f;

    private float defaultFov; 

    Rigidbody rb;

    public static PlayerController instance;


    private bool isGrounded;
    private bool isJumping;

    private bool isAttacking;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EventBus<SwordHitEvent>.OnEvent += TakeHit;

        defaultFov = cam.fieldOfView;

        isGrounded = true;
        isJumping = false;
        canDash = true;
        isDashing = false;
        isAttacking = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        Move();
        Interact();
        JumpLogic();

        //Delete after 
        if (Input.GetKeyDown(KeyCode.X))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (stamina.currentStamina <= 10) return;

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Light Attack");
        }

        // Gradually return FOV to default if not dashing
        if (canDash && Mathf.Abs(cam.fieldOfView - defaultFov) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, Time.deltaTime * fovChangeSpeed);
        }
    }

    public void StartAttack()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", isAttacking);
    }

    public void StopAttack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
    }

    public void FinalAttack()
    {
        //Prevent instantly attacking after last one
        anim.ResetTrigger("Light Attack");
    }

    private void Interact()
    {
        if (Input.GetKey (KeyCode.Tab)) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, pickupRange);
            
            if (colliders.Length <= 0) return;

            Interactable closestInteractable = null;
            float closestDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                Interactable interactable = collider.gameObject.GetComponent<Interactable>();


                if (interactable != null)
                {
                    float distance = Vector3.Distance(this.transform.position, interactable.transform.position);
                    
                    if (distance < interactable.radius && distance < closestDistance)
                    {
                        closestInteractable = interactable;
                        closestDistance = distance;
                    }
                }
            }

            if (closestInteractable != null)
            {
                closestInteractable.interacted = true;
            }
        }
    }
    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position + new Vector3 (0, this.GetComponent<Collider> ().bounds.size.y / 2, 0), pickupRange);
    }

    private void JumpLogic()
    {
        //Dogshit function

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            anim.SetTrigger("Jump");
        }

        
    }

    private IEnumerator roll(Vector3 direction)
    {
        anim.SetTrigger("Roll");
        stamina.ChangeStamina(10); // Reduce stamina after dash

        isDashing = true;
        canDash = false;

        // Increase FOV for dash effect
        float targetFov = defaultFov + dashFovIncrease;

        // Apply initial dash force
        rb.velocity = Vector3.zero; // Reset velocity for clean dash

        if(direction == Vector3.zero)
        {
            direction = transform.forward;
        }

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

        isDashing = false;

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
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        isJumping = true;
    }



    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        input.Normalize();

        Vector3 cameraRight = cam.transform.right;
        Vector3 cameraForward = Vector3.Cross(cameraRight, Vector3.up);
        Vector3 move = input.x * cameraRight + input.z * cameraForward;

        transform.Translate(move * speed * Time.deltaTime, Space.World);        


        Quaternion rotation = transform.rotation;
        Quaternion targetRotation;

        if(input != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(input.x * cameraRight + input.z * cameraForward);
        }
        else if(input.z < 0.0f)
        {
            targetRotation = Quaternion.LookRotation(-cameraForward);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(transform.forward);
        }

        transform.rotation = Quaternion.Lerp(rotation, targetRotation, Time.deltaTime * rotationSpeed);

        Vector3 dashDirection = (input.x * cameraRight + input.z * cameraForward).normalized;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(roll(dashDirection));

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity;


        if (Physics.Raycast(cam.transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            if (hitInfo.distance <= 1f)
            {
                //Camera can move up but not down
                mouseY = Mathf.Max(mouseY, 0.0f);
            }
        }
        else
        {
            Physics.Raycast(cam.transform.position, Vector3.up, out RaycastHit hit);
            cam.transform.position = new Vector3(cam.transform.position.x, hit.point.y + 1.0f, cam.transform.position.z);
        }

        // Clamp rotation so you don't go over the player
        float dotProduct = Vector3.Dot(cam.transform.forward, Vector3.down);

        //Camera can move down but not up
        if (dotProduct >= 0.75f) mouseY = Mathf.Min(mouseY, 0.0f);

        // Rotate around the player
        cam.transform.RotateAround(transform.position, Vector3.up, mouseX);
        cam.transform.RotateAround(transform.position, cam.transform.right, mouseY);
        cam.transform.position = transform.position + cam.transform.rotation * cameraOffset;


        anim.SetFloat("Movement", input.magnitude);
    }

    void TakeHit(SwordHitEvent pEvent)
    {
    }

    void Attack()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Terrain"))
        {
            if (isJumping && !isGrounded)
            {
                isGrounded = true;
                isJumping = false;
                anim.SetTrigger("Land");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Terrain"))
        {
            isGrounded = false;
        }
    }
}

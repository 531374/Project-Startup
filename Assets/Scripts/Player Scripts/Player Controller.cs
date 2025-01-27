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

    [Header ("Attack Setting")]
    [SerializeField] private float normalAttackStaminaCost = 2f;
    [SerializeField] private float heavyAttackStaminaCost = 7f;

    [Header("Roll Settings")]
    [SerializeField] private float rollPower = 24f;
    [SerializeField] private float rollDuration = 1f;
    [SerializeField] private float rollStaminaCost = 10f;

    [HideInInspector] public bool isRolling;
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

    public bool isAttacking;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EventBus<SwordHitEvent>.OnEvent += TakeHit;

        defaultFov = cam.fieldOfView;

        isGrounded = true;
        isJumping = false;
        canDash = true;
        isRolling = false;
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
        ShowHideCursor ();
        Attack ();

        if (canDash && Mathf.Abs(cam.fieldOfView - defaultFov) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, Time.deltaTime * fovChangeSpeed);
        }
    }

    //Delete after 
    private void ShowHideCursor ()
    {
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
    }

    private void Attack ()
    {
        if (Input.GetMouseButtonDown(0) && stamina.currentStamina > normalAttackStaminaCost)
        {
            stamina.TakeStamina (normalAttackStaminaCost);
            anim.SetTrigger("Light Attack");
        }

        if (Input.GetMouseButtonDown(1) && stamina.currentStamina > heavyAttackStaminaCost)
        {
            stamina.TakeStamina (heavyAttackStaminaCost);
            anim.SetTrigger("Heavy Attack");
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
        anim.ResetTrigger("Heavy Attack");
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
        
        if (stamina.currentStamina <= rollStaminaCost) 
        {
            yield return null;
        }
        Debug.Log (isRolling);
        anim.applyRootMotion = false;
        anim.SetTrigger("Roll");
        stamina.TakeStamina(rollStaminaCost);

        isRolling = true;
        canDash = false;

        float targetFov = defaultFov + dashFovIncrease;

        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(direction * rollPower, ForceMode.Impulse);

        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovChangeSpeed);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        anim.applyRootMotion = true;
        isRolling = false;
        canDash = true;

        rb.velocity = Vector3.zero;

        while (Mathf.Abs(cam.fieldOfView - defaultFov) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, Time.deltaTime * fovChangeSpeed);
            yield return null;
        }

        cam.fieldOfView = defaultFov;
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

        if (!isAttacking)
        {
            transform.Translate(move * speed * Time.deltaTime, Space.World);        
        }


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
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isAttacking) StartCoroutine(roll(dashDirection));

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

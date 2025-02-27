using FMODUnity;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerHealthManager))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] Animator anim;
    [SerializeField] Stamina stamina;
    [SerializeField] Transform staminaImage;
    [SerializeField] GameObject ship;

    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float jumpStrength;
    [SerializeField] private float rotationSpeed;

    [Header("Attack Setting")]
    [SerializeField] private float baseDamage = 10.0f;
    [SerializeField] private float heavyAttackDamageModifier = 1.5f;
    [SerializeField] private float normalAttackStaminaCost = 5f;
    [SerializeField] private float heavyAttackStaminaCost = 10f;

    [SerializeField] private StudioEventEmitter LightAttackSoundEmitter;
    [SerializeField] private StudioEventEmitter HeavyAttackSoundEmitter;

    [Header("Roll Settings")]
    [SerializeField] private float rollPower = 24f;
    [SerializeField] private float rollDuration = 1f;
    [SerializeField] private float rollStaminaCost = 15f;

    [HideInInspector] public bool isRolling;
    private bool canDash;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float dashFovIncrease = 20f; // Amount by which FOV increases during dash
    [SerializeField] private float fovChangeSpeed = 5f;   // Speed of FOV change

    [Header ("Interactable Settings")]
    [SerializeField] public float pickupRange;

    public float damage;

    private float defaultFov; 

    Rigidbody rb;

    public static PlayerController instance;

    private PlayerHealthManager health;


    private bool isGrounded;
    private bool isJumping;

    public bool isAttacking;

    public bool isEnabled;
    public bool canBeHit;

    private Vector3 input;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        defaultFov = cam.fieldOfView;

        isGrounded = true;
        isJumping = false;
        canDash = true;
        isRolling = false;
        isAttacking = false;

        Cursor.lockState = CursorLockMode.Locked;

        if (ship != null)
        isEnabled = !ship.GetComponent<ShipController>().isEnabled;

        canBeHit = true;

        damage = baseDamage;

    }

    private void Awake()
    {
        health = GetComponent<PlayerHealthManager>();
        if (instance == null) instance = this;
    }

    private void Update()
    {
        //if (!isEnabled) return;
        
        if (Input.GetKeyDown(KeyCode.E) && isEnabled)
        {
            if(Vector3.Distance(transform.position, ship.transform.position) < 25.0f)
            {
                isEnabled = false;
                ship.GetComponent<ShipController>().isEnabled = true;

                stamina.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }


        Move();
        Interact();
        //JumpLogic();
        ShowHideCursor ();
        Attack ();


        if (canDash && Mathf.Abs(cam.fieldOfView - defaultFov) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, Time.deltaTime * fovChangeSpeed);
        }
    }


    //Now dont delete this
    private void ShowHideCursor ()
    {
        if (BookManager.instance.isBookOpened && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!BookManager.instance.isBookOpened && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Attack ()
    {
        if (Input.GetMouseButtonDown(0) && stamina.currentStamina > normalAttackStaminaCost)
        {
            anim.SetTrigger("Light Attack");
        }

        if (Input.GetMouseButtonDown(1) && stamina.currentStamina > heavyAttackStaminaCost)
        {
            anim.SetTrigger("Heavy Attack");
        }
    }

    public void StartLightAttack()
    {
        damage = baseDamage;
        anim.applyRootMotion = true;
        stamina.TakeStamina (normalAttackStaminaCost);
        isAttacking = true;
        anim.SetBool("isAttacking", isAttacking);
        if (LightAttackSoundEmitter != null)
        {
            LightAttackSoundEmitter.Play();
        }

    }

    public void StartHeavyAttack()
    {
        damage = baseDamage * heavyAttackDamageModifier;
        anim.applyRootMotion = true;
        stamina.TakeStamina (heavyAttackStaminaCost);
        isAttacking = true;
        anim.SetBool("isAttacking", isAttacking);
        if (HeavyAttackSoundEmitter != null)
        {
            HeavyAttackSoundEmitter.Play();
        }
    }


    public void StopAttack()
    {
        damage = baseDamage;
        anim.applyRootMotion = false;
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
    }

    public void FinalAttack()
    {
        //Prevent instantly attacking after last one
        anim.ResetTrigger("Light Attack");
        anim.ResetTrigger("Heavy Attack");
    }

    public void ResetTriggerCollider ()
    {
        anim.SetBool ("CanCollide", true);
    }

    

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown (KeyCode.Tab))
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
            //anim.SetTrigger("Jump");
        }
    }

    private IEnumerator roll(Vector3 direction)
    {
        
        if (stamina.currentStamina <= rollStaminaCost) 
        {
            yield return null;
        }

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

    IEnumerator jumpCooldown()
    {
        //Set to true after 10 frames
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }

        isJumping = true;
    }


    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        isJumping = true;
        //StartCoroutine(jumpCooldown());
    }



    void Move()
    {
        if (BookManager.instance.isBookOpened) return;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        input.Normalize();

        Vector3 cameraRight = cam.transform.right;
        Vector3 cameraForward = Vector3.Cross(cameraRight, Vector3.up);
        this.input = input.x * cameraRight + input.z * cameraForward;

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
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isAttacking && stamina.currentStamina > rollStaminaCost) StartCoroutine(roll(dashDirection));

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


        
    }

    private void FixedUpdate()
    {

        if (!isAttacking && !isJumping)
        {
            float speedModifier = 1f;
            if (stamina.currentStamina < 10.0f) speedModifier = 0.5f;

            Vector3 movement = input * (speed * speedModifier) * Time.fixedDeltaTime;

            if (rb.SweepTest(movement.normalized, out RaycastHit hit, movement.magnitude))
            {
                // Project movement onto the collision surface (sliding effect)
                Vector3 slideDirection = Vector3.ProjectOnPlane(movement, hit.normal);
                rb.MovePosition(rb.position + slideDirection);
            }
            else
            {
                rb.MovePosition(rb.position + movement);
            }


            //rb.MovePosition(rb.position + input * (speed * speedModifier) * Time.fixedDeltaTime);

            anim.SetFloat("Movement", input.normalized.magnitude * speedModifier);
        }

    }

    // private void OnDisable()
    // {
    //     Cursor.lockState = CursorLockMode.None;
    // }

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

    private void OnTriggerEnter(Collider other)
    {
        if (isRolling) return;

        if (other.transform.CompareTag("Leg"))
        {
            if (other.transform.GetComponentInParent<EnemyController>().isAttacking && canBeHit)
            {
                health.TakeDamage(10);
                canBeHit = false;
            }
        }

        if (other.transform.CompareTag("Stinger"))
        {
            if (other.transform.GetComponentInParent<EnemyController>().isAttacking && canBeHit)
            {
                health.TakeDamage(25);
                canBeHit = false;
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

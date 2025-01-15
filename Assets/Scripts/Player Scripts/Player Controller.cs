using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;

    [Header("Movement Settings")]
    public float speed;
    public float sensitivity;

    [Header("Dash Settings")]
    public float dashingPower = 24f;
    public float dashCooldown = 2.5f;
    public float dashDuration = 0.75f;
    private bool canDash = true;
    private bool isDashing = false;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(); 
    }

    private IEnumerator dash(Vector3 direction)
    {
        canDash = false;
        isDashing = true;
        rb.AddForce(direction * dashingPower, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;        
    }

    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        float dx = input.x * speed * Time.deltaTime;
        float dz = input.z * speed * Time.deltaTime;
        transform.Translate(dx, 0, dz);

        //Dash in players forward direction
        Vector3 dashDirection = (input.x * transform.right + input.z * transform.forward).normalized;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(dash(dashDirection));

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity;

        //Horizontal rotation
        transform.Rotate(0, mouseX, 0);

        //Make camera not go into the ground
        if(Physics.Raycast(cam.transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            if(hitInfo.distance < 0.5f)
            {
                mouseY = Mathf.Max(mouseY, 0.0f);
            }
        }

        //Clamp rotation so you don't go over the player
        float dotProduct = Vector3.Dot(cam.transform.forward, Vector3.down);
        if (dotProduct >= 0.75f) mouseY = Mathf.Min(mouseY, 0.0f);

        //Rotate around the player up and down
        cam.transform.RotateAround(transform.position, transform.right, mouseY);
    }
}

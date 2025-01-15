using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Camera cam;
    public float speed;
    public float sensitivity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float dx = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float dz = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(dx, 0, dz);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureAnim : MonoBehaviour
{
    private PlayerController player;
    private Camera cam;
    [SerializeField] private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
        cam = Camera.main;
    }


    public void OnAnimationStart ()
    {
        player.enabled = false;
        cam.transform.LookAt (transform.position + offset);
        Time.timeScale = 0;
    }

    public void OnAnimationEnd ()
    {
        player.enabled = true;
        Time.timeScale = 1;
    }
}

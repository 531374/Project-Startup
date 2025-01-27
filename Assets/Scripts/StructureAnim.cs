using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureAnim : MonoBehaviour
{
    private PlayerController player;
    private Camera cam;
    [SerializeField] private Vector3 offset; // Offset from the object
    private bool isAnimTriggered;
    private bool hasAnimEnded;
    [SerializeField] private float lerpSpeed = 1.0f; // Lerp speed
    private Vector3 camStartPos;
    private Quaternion camStartRotation;

    private const float positionThreshold = 0.1f;
    private const float rotationThreshold = 0.01f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cam = Camera.main;
    }

    void Update()
    {
        // Animation-in progress: Rotate the camera toward the target
        if (isAnimTriggered)
        {
            Quaternion camRotation = cam.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(transform.GetChild(0).position - cam.transform.position);
            cam.transform.rotation = Quaternion.Lerp(camRotation, targetRotation, Time.unscaledDeltaTime * lerpSpeed);
        }

        // Restore camera position and rotation when the animation ends
        if (hasAnimEnded)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, camStartPos, Time.unscaledDeltaTime * lerpSpeed / 2);
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, camStartRotation, Time.unscaledDeltaTime * lerpSpeed / 2);

            // Check if the camera is close enough to the target position and rotation
            if (Vector3.Distance(cam.transform.position, camStartPos) < positionThreshold &&
                Quaternion.Angle(cam.transform.rotation, camStartRotation) < rotationThreshold)
            {
                hasAnimEnded = false;
                player.enabled = true;
                Time.timeScale = 1;
            }
        }
    }

    public void OnAnimationStart()
    {
        player.enabled = false;
        Time.timeScale = 0;
        isAnimTriggered = true;

        camStartPos = cam.transform.position;
        camStartRotation = cam.transform.rotation;
    }

    public void OnAnimationEnd()
    {
        hasAnimEnded = true;
        isAnimTriggered = false;
    }
}

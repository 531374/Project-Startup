using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        SwordHitEvent hitEvent = new SwordHitEvent(other.transform, transform.parent);
        EventBus<SwordHitEvent>.Publish(hitEvent);
    }
}

public class SwordHitEvent : Event
{
    public readonly Transform hitTransform;
    public readonly Transform parent;

    public SwordHitEvent(Transform hitTransform, Transform parent)
    {
        this.hitTransform = hitTransform;
        this.parent = parent;
    }
}
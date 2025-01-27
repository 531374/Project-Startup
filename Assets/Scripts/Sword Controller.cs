using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerController playerController;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController == null) playerController = PlayerController.instance;
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerController.isAttacking) return;
        SwordHitEvent hitEvent = new SwordHitEvent(other.transform, playerController.transform);
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
        GetHealthComponent (hitTransform, parent);
    }

    public void GetHealthComponent (Transform target, Transform parent)
    {
        if (target.tag == "Player")  target.GetComponent<PlayerHealthManager> ().TakeDamage (parent.GetComponent<EnemyController>().damage);
        else if (target.tag == "Enemy") target.GetComponent <EnemyHealthMananger> ().TakeDamage (parent.GetComponent <PlayerController>().damage);
    }
}
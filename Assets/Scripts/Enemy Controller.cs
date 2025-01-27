using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof (EnemyHealthMananger))]
public class EnemyController : MonoBehaviour
{


    [Header("References")]
    [SerializeField] Animator anim;

    [Header("Enemy stats")]
    public float detectionRange = 50.0f;
    public float attackRange = 5.0f;
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float damage = 5f;


    private bool detectedPlayer;

    PlayerController player;

    EnemyHealthMananger health;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.instance;
        agent = GetComponent<NavMeshAgent>();
        
        detectedPlayer = false;
        agent.speed = speed;

        EventBus<SwordHitEvent>.OnEvent += CheckHit;

        health = GetComponent<EnemyHealthMananger>();   
    }

    // Update is called once per frame
    void Update()
    {

        float playerDst = Vector3.Distance(player.transform.position, transform.position);  

        if(!detectedPlayer && playerDst < detectionRange)
        {
            detectedPlayer = true;
        }

        if (detectedPlayer && playerDst > attackRange)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        if(playerDst <= attackRange)
        {
            agent.SetDestination(transform.position);

            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo))
            {
                if(hitInfo.transform.name == "Player")
                {
                    anim.SetTrigger("Swing");
                }
            }
        }

        anim.SetFloat("Move", agent.velocity.magnitude);

    }

    void CheckHit(SwordHitEvent pEvent)
    {
        if(pEvent.hitTransform == this.transform)
        {
            health.TakeDamage(10f);
            if(health.currentHealth <= 0f)
            {
                Debug.Log("Enemy died!");
                Destroy(gameObject);
            }
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

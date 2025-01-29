using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealthMananger))]
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
    public float attackCooldown = 2.0f;

    private bool detectedPlayer;

    private bool playerInAttackRange;

    PlayerController player;
    EnemyHealthMananger health;
    NavMeshAgent agent;

    float lastAttack;
    public bool isAttacking;


    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.instance;
        agent = GetComponent<NavMeshAgent>();

        detectedPlayer = false;
        agent.speed = speed;

        EventBus<SwordHitEvent>.OnEvent += CheckHit;

        health = GetComponent<EnemyHealthMananger>();

        playerInAttackRange = false;

        isAttacking = false;
        lastAttack = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isAttacking + " " + PlayerController.instance.canBeHit);

        if (!detectedPlayer && playerInAttackRange)
        {
            detectedPlayer = true;
        }

        if (detectedPlayer && !playerInAttackRange)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        if (playerInAttackRange)
        {
            agent.SetDestination(transform.position);

            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo) && playerInAttackRange)
            {
                if (hitInfo.transform.name == "Player")
                {
                    AttackLogic();
                }
            }
        }

        AnimationLogic();

    }

    void AttackLogic()
    {
        if(!isAttacking && Time.time - lastAttack >= attackCooldown)
        {
            //isAttacking = true;
            int random = Random.Range(0, 2);

            if (random == 0) anim.SetTrigger("Sting");
            if (random == 1) anim.SetTrigger("Slash");
        }
    }

    public void SwitchLegs()
    {
        PlayerController.instance.canBeHit = true;
    }

    public void StartAttack()
    {
        isAttacking = true;
    }

    public void StopAttack()
    {
        isAttacking = false;
        PlayerController.instance.canBeHit = true;
        lastAttack = Time.time;
    }

    void AnimationLogic()
    {
        float playerDst = Vector3.Distance(player.transform.position, transform.position);

        if (!playerInAttackRange && playerDst <= attackRange)
        {
            anim.SetTrigger("PlayerInRange");
            anim.ResetTrigger("PlayerOutOfRange");
            playerInAttackRange = true;
        }

        if (playerInAttackRange && playerDst > attackRange + 1.0f)
        {
            anim.SetTrigger("PlayerOutOfRange");
            anim.ResetTrigger("PlayerInRange");
            playerInAttackRange = false;
        }

        anim.SetFloat("Move", agent.velocity.magnitude);
    }

    void CheckHit(SwordHitEvent pEvent)
    {
        // if (pEvent.hitTransform == this.transform)
        // {
        //     health.TakeDamage(10f);
        //     if (health.currentHealth <= 0f)
        //     {
        //         Destroy(gameObject);
        //     }
        // }
    }

    private void OnDisable()
    {
        EventBus<SwordHitEvent>.OnEvent -= CheckHit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

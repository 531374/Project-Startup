using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject keyCapPrefab;
    [SerializeField] private Vector3 offset = new Vector3 (0, 10, 0);
    public float radius = 3f;
    [HideInInspector] public bool interacted;

    protected Transform player;
    protected GameObject keyCap;

    private void Start ()
    {
        
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, radius);
    }

    protected virtual void Update ()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform ?? GameObject.FindGameObjectWithTag("Ship").transform;
        }

        Interact ();
        ShowKeyCap ();
    }

    protected virtual void Interact ()
    {
        
    }

    private void ShowKeyCap ()
    {   
        if (keyCap == null && Vector3.Distance (transform.position, player.position) < radius)
        {
            keyCap = Instantiate (keyCapPrefab, transform.position + offset, Quaternion.identity, transform.GetChild(0).transform);
        } else if (keyCap != null && Vector3.Distance (player.position, transform.position) > radius)
        {
            Destroy (keyCap);
        }

        if (keyCap != null) keyCap.transform.LookAt (Camera.main.transform.position);
    }
}

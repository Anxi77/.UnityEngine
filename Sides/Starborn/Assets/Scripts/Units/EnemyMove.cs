using SpaceShooter;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private GameObject player;
    public float moveSpeed = 22f;
    Vector3 direction;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {

    }

    void Update()
    {
        direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(30f, 0f, angle - 90f);
        transform.rotation = targetRotation;
        if (player != null && direction != null)
        {            
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player) 
        {
            Destroy(player);
        }
        else 
        {
            DestroyImmediate(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
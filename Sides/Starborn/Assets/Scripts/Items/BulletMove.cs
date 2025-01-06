using Unity.Mathematics;
using UnityEngine;

/*
public class BulletMove : MonoBehaviour
{
    public float velocity = 10f;
    public float lifeTime = 2f;
    private GameObject player;
    private Vector3 direction;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Start()
    {
        
    }

    private void Update()
    {
        transform.localRotation = player.transform.localRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
*/


public class BulletMove : MonoBehaviour
{
    public float velocity = 0.1f;
    private Vector3 direction;
    public float lifeTime = 2f;
    private void Start()
    {
        SetDirection();
    }

    private void SetDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        transform.rotation = targetRotation;
    }

    private void Update()
    {
        transform.position += direction * velocity * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            print("충돌 발생!!");
        }
    }
}
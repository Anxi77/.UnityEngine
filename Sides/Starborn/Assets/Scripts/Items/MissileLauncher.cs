using UnityEngine;
using System.Collections;
using TMPro;

public class MissileLauncher : MonoBehaviour
{
    public GameObject Bullet;
    public float fireRate = 5f;
    public int magazineSize = 30;
    private int magazine;
    private GameObject player;
    public float reloadTime = 0f;
    private bool isFiring = false;
    private Coroutine missileFire;
    public TextMeshProUGUI Text;

    private void Awake()
    {
        magazine = magazineSize;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerMove");
        if (Text != null)
        {
            Text.gameObject.SetActive(true);
        }
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFiring)
        {
            missileFire = StartCoroutine(FireBullets());
        }
        else if (Input.GetMouseButtonUp(0) && isFiring) 
        {
            isFiring = false;   
            StopCoroutine(missileFire);
        }
    }

    public IEnumerator FireBullets()
    {
        print("Fire !!");
        isFiring = true;

        while (true)
        {
            Text.gameObject.SetActive(true);
            Text.text = $"Magazine : {magazine}";
            if (magazine <= 0)
            {
                StartCoroutine(Reloading());
                yield return new WaitForSecondsRealtime(reloadTime);
                StopCoroutine(Reloading());
            }

            if (player != null)
            {
                Instantiate(Bullet, transform.position, Quaternion.identity);
                magazine--;
            }
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

    public IEnumerator Reloading() 
    {
        if (Text != null)
        {
            float remainingTime = reloadTime;
            while (remainingTime > 0)
            {
                Text.text = $"Reloading... {remainingTime:F1}s";
                yield return new WaitForSeconds(Time.deltaTime);
                remainingTime -= Time.deltaTime;
            }
            magazine = magazineSize;
            Text.gameObject.SetActive(false);
        }
    }

}
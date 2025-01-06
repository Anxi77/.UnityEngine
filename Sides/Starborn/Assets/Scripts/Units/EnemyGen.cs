using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGen : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    public float spawnInterval = 1f;
    private float nextSpawnTime;
    private GameObject[] spawnable;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
        spawnable = GameObject.FindGameObjectsWithTag("EnemySpawnPos");

    }

    void Update()
    {
        int randompos = Random.Range(0, spawnable.Length);


        if (Time.time >= nextSpawnTime && spawnable.Length > 0)
        {

            GameObject randomSpawnable = spawnable[randompos];

            Instantiate(Enemy, randomSpawnable.transform.position, Quaternion.identity);


            nextSpawnTime = Time.time + spawnInterval;

            print($"적이 생성되었습니다: {randomSpawnable.transform.position} ");

            print($"스포너블 번호 : {randompos}");
        }
    }
}
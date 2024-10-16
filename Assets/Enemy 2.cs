using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    public NavMeshAgent Enamy;
    public Transform Player;
    public Animator animator;
    public float enemySpeed = 1.5f;

    public GameObject StandingEffect;
    public GameObject enemyPrefab;  
    public Transform[] spawnPoints;  
    public int maxEnemies = 2;  
    public float spawnDelay = 5f;
    private int currentEnemyCount = 0;

    private float distanceToTarget = Mathf.Infinity;
    private bool isProvoked = false;

    void Start()
    {  
        Enamy = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Enamy.speed = enemySpeed;
        animator.SetTrigger("Idle");
        StandingEffect.SetActive(true);

        StartCoroutine(SpawnEnemiesWithDelay());
    }

    void Update()
    {
        Enamy.SetDestination(Player.position);
    }
    IEnumerator SpawnEnemiesWithDelay()
    {
        if (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    // Method to spawn a single enemy at a random spawn point
    private void SpawnEnemy()
    {
        // Select a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);

        // Instantiate the enemy at the selected spawn point
        Instantiate(enemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);

        // Increment the current enemy count
        currentEnemyCount++;
    }

    // Method to handle when an enemy is defeated
    public void OnEnemyDefeated()
    {
        // Decrement the current enemy count
        currentEnemyCount--;

        // Ensure it doesn't go below zero (in case of any errors)
        if (currentEnemyCount < 0)
        {
            currentEnemyCount = 0;
        }
    }
}
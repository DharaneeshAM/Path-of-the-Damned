using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    public NavMeshAgent Enamy;
    public Transform Player;
    public Animator animator;
    public float enemySpeed = 1.5f;
    public float chaseRange = 5.0f;

    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;

    public GameObject StandingEffect;

    public AudioSource EnemySource;
    public AudioClip Stanging;


    void Start()
    {
        Enamy = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Enamy.speed = enemySpeed;
        animator.SetTrigger("Idle");
        StandingEffect.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(Player.position, transform.position);
        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange)
        {
            isProvoked = true;
            EnemySource.clip = Stanging;
            EnemySource.Play();
        }
    }
    private void EngageTarget()
    {
        if (distanceToTarget >= Enamy.stoppingDistance)
        {
            Enamy.SetDestination(Player.position);
            animator.SetTrigger("Atack");
            StandingEffect.SetActive(false);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
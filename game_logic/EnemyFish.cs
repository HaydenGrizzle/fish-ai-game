using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class EnemyFish : MonoBehaviour
{
    public float mass = 2f;
    public float moveSpeed = 2f;
    public float detectionRange = 12f;
    public float eatRange;
    public float growthAmount = 0.3f;
    public float dur = 0.2f;
    public Transform[] wanderPoints;
    public ParticleSystem enemyParticle;

    private Transform target;
    private Transform currentWanderPoint;
    private float wanderTimer;

    private Transform[] holder;


    void Start()
    {
        //---------------
        // Start Stats
        //---------------
        mass = Random.Range(0.2f, 0.5f);
        moveSpeed = Random.Range(2f, 4f); // CHECK
        eatRange = mass + 1f;
        transform.localScale = Vector3.one * mass; // GROWTH STUFF


        //--------------------
        // Dynamic Way Points
        //--------------------
        GameObject[] wp = GameObject.FindGameObjectsWithTag("waypoint");
        for (int i=0; i < wp.Length; i++)
        {
            wanderPoints[i] = wp[i].transform;
        }
    }


    void Update()
    {
        FindTarget();

        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            // Smooth rotation toward movement direction
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
        }

        else
        {
            Wander();
        }
    }


    void FindTarget()
    {
        float closestDist = Mathf.Infinity;
        Transform bestTarget = null;

        // --- 1️⃣ Find other enemies ---
        EnemyFish[] allFish = FindObjectsOfType<EnemyFish>();
        foreach (var f in allFish)
        {
            if (f == this) continue;
            float dist = Vector3.Distance(transform.position, f.transform.position);

            if (dist < detectionRange && f.mass < mass && dist < closestDist)
            {
                closestDist = dist;
                bestTarget = f.transform;
            }
        }

        // --- 2️⃣ Include the player fish ---
        FihState player = FindObjectOfType<FihState>();
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < detectionRange && player.GetMass() < mass && dist < closestDist)
            {
                closestDist = dist;
                bestTarget = player.transform;
            }
        }

        target = bestTarget;
    }


    void Eat(float eatenMass)
    {
        StartCoroutine(SmoothGrow(mass + eatenMass * 0.5f, dur));

        //float size = transform.localScale.x;
        //eatRange = size + (size * 0.8f);
        //detectionRange = size * 6f;
    }


    IEnumerator SmoothGrow(float targetMass, float duration)
    {
        float startMass = mass;
        float elapsed = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * targetMass; // GROWTH STUFF

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mass = Mathf.Lerp(startMass, targetMass, elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }

        // update ranges at end
        float size = transform.localScale.x;
        eatRange = size * 1.2f;
        enemyParticle.Play();
        Debug.Log("ATE");
        //detectionRange = size * 6f;
    }


    void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (currentWanderPoint == null || wanderTimer > 15f)
        {
            wanderTimer = 0f;
            currentWanderPoint = wanderPoints[Random.Range(0, wanderPoints.Length)];
        }

        if (currentWanderPoint != null)
        {
            Vector3 dir = (currentWanderPoint.position - transform.position).normalized;
            transform.position += dir * moveSpeed * 0.5f * Time.deltaTime;

            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 3f * Time.deltaTime);
            }

            // Too close to point
            if (Vector3.Distance(transform.position, currentWanderPoint.position) < 2f)
            {
                currentWanderPoint = wanderPoints[Random.Range(0, wanderPoints.Length)];
                wanderTimer = 0f;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            FihState fih = other.GetComponent<FihState>();
            if (fih != null && fih.GetMass() < mass)
            {
                Eat(fih.GetMass());
                Destroy(other.gameObject);
                return;
            }

            EnemyFish enemy = other.GetComponent<EnemyFish>();
            if (enemy != null && enemy.mass < mass)
            {
                Eat(enemy.mass);
                Destroy(other.gameObject);
                return;
            }
        }
    }
}
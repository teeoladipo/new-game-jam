using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEditor.PackageManager;
using UnityEngine;

public class Boss1Controller : EnemyController
{
    public enum State {
        Idle,
        Move_front,
        Move_back
    }

    public State currentState;

    public float Speed = 1f;

    public float AverageAttackRate = 1.5f;
    public float AttackRateSpread = 0.5f;

    public EnemyProjectileBehavior ProjectilePrefab;
    public Transform LaunchOffset1;
    public Transform LaunchOffset2;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(PerformState());
        StartCoroutine(FireProjectile());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == State.Move_front) {
            transform.position += new Vector3(Mathf.Sign(player.transform.position.x - transform.position.x), 0, 0) * Time.deltaTime * Speed;
        }
        else if(currentState == State.Move_back) {
            transform.position += new Vector3(Mathf.Sign(transform.position.x - player.transform.position.x), 0, 0) * Time.deltaTime * Speed;
        }

        transform.rotation = player.transform.position.x < transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    public IEnumerator PerformState() {
        float waitSecond = Random.Range(1f, 3f);
        yield return new WaitForSeconds(waitSecond);
        currentState = (State) Random.Range(0, 3);
        StartCoroutine(PerformState());
    }

    public IEnumerator FireProjectile(){
        float waitSecond = Random.Range(AverageAttackRate - AttackRateSpread, AverageAttackRate + AttackRateSpread);
        yield return new WaitForSeconds(waitSecond);

        if(Random.Range(0,2) == 0) {
            Instantiate(ProjectilePrefab, LaunchOffset1.position, transform.rotation);
        }
        else {
            Instantiate(ProjectilePrefab, LaunchOffset2.position, transform.rotation);
        }
        StartCoroutine(FireProjectile());
    }
}

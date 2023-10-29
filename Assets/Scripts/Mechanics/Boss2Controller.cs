using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Boss2Controller : EnemyController
{
    public enum State {
        Idle,
        Charge,
        Attack,
        Stuck,
        Move_back
    }

    public State currentState;

    public float Speed = 1f;

    public float chargeSpeed = 3f;

    public float moveBackTime = 2f;
    public float attackTime = 1f;
    public float stuckTime = 3f;

    public Transform AttackPoint;

    public float detectRange = 1.5f;
    public float attackRange = 2.0f;
    public float damage = 3f;

    public LayerMask playerLayer;

    public bool touchingWall = false;


    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(PrepareAttack());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == State.Charge) {
            if(!touchingWall)
                transform.position += new Vector3(Mathf.Sign(player.transform.position.x - transform.position.x), 0, 0) * Time.deltaTime * chargeSpeed;
            if(Mathf.Abs(transform.position.x - player.transform.position.x) < attackRange * 2) {
                StartCoroutine(Attack());
            }
        }
        else if(currentState == State.Move_back) {
            control.SetTrigger("idle");
            if(!touchingWall)
                transform.position += new Vector3(Mathf.Sign(transform.position.x - player.transform.position.x), 0, 0) * Time.deltaTime * Speed;
        }

        transform.rotation = player.transform.position.x < transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    /*public void PlayerDetected()
    {
        StartCoroutine(Attack());
    }*/

    public IEnumerator PrepareAttack(){
        currentState = State.Move_back;
        yield return new WaitForSeconds(moveBackTime);
        currentState = State.Charge;
        control.SetTrigger("charge");
    }

    public IEnumerator Attack() {
        currentState = State.Attack;
        yield return new WaitForSeconds(attackTime);
        control.SetTrigger("attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, playerLayer);
        if(hitEnemies.Length > 0) {
            player.Damage(damage);
        }
        currentState = State.Stuck;
        yield return new WaitForSeconds(stuckTime);
        StartCoroutine(PrepareAttack());
    }

    void OnDrawGizmosSelected() {
        if(AttackPoint == null)
            return;
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "PlayerProjectile") {
            health.Decrement(player.projectileDamage);
            Destroy(collision.gameObject);
        }
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Wall")) {
            touchingWall = true;
        }
    }

    void OnCollisionExit2D(Collision2D other){
        if(other.gameObject.CompareTag("Wall")) {
            touchingWall = false;
        }
    }
}

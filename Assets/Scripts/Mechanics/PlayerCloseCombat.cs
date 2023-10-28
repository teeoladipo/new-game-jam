using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCloseCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int MeleeDamage = 1;
    public float attackRate = 0.25f;
    public LayerMask enemyLayers;

    public Health health;

    bool attackEnabled = true;

    void Start() {
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) { // mouse button
            Attack();
        }
    }

    void Attack() {
        if(!attackEnabled || !health.IsAlive)
            return;
        
        animator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies) {
            enemy.gameObject.GetComponent<Health>().Decrement(MeleeDamage);
        }

        StartCoroutine(AttackCooldown());
    }

    public IEnumerator AttackCooldown() {
        attackEnabled = false;
        yield return new WaitForSeconds(attackRate);
        attackEnabled = true;
    }

    void OnDrawGizmosSelected() {
        if(attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

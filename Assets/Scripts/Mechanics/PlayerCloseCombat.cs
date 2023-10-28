using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCloseCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) { // mouse button
            Attack();
        }
    }

    void Attack() {
        animator.SetTrigger("attack");

        Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
    }
}

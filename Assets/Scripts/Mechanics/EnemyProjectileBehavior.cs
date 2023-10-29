using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    public float speed = 4.5f;

    public float damage = 2f;

    public PlayerController player;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("PlayerProjectile")) {
            if(collision.gameObject.CompareTag("Player")){
                player.Damage(damage);
            }
            Destroy(gameObject);
        }
    }
}

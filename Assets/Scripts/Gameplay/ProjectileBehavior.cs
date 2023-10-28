using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 4.5f;
    public int damage = 1;


    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed; 

    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag != "Player")
            Destroy(gameObject); 
    }
}

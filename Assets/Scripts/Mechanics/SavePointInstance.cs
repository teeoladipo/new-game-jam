using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class SavePointInstance : MonoBehaviour
{
    public bool Enabled = false;
    public Transform spot;

    public PlayerController player;

    public bool PlayerInTrigger = false;

    SpriteRenderer sprite;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && PlayerInTrigger) {
            Enable();
        }
        /*else if(player.SavePoint != spot) {
            Disable();
        }*/
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) {
            PlayerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            PlayerInTrigger = false;
        }
    }

    void Enable() {
        player.SavePoint = spot;
        Enabled = true;
        sprite.color = new Color(1, 1, 1, 1);
    }

    void Disable() {
        Enabled = false;
        sprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2AttackTriggerBehavior : MonoBehaviour
{

    public Boss2Controller controller;

    void OnTriggerStay2D(Collider2D collision) {
        
        if(controller.currentState == Boss2Controller.State.Charge && collision.gameObject.CompareTag("Player")) {
            //controller.PlayerDetected();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class ChipInstance : MonoBehaviour
{

    public enum type{
        HEALTH,
        DEFENSE,
        STAT_PROJ_SPD,
        STAT_PROJ_DMG,
        STAT_ATK_SPD,
        STAT_ATK_DMG,
        STAT_MVMT_SPD
    }

    public type Type = type.HEALTH;

    bool collected = false;

    void Start() {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        switch(Type) {
            case type.HEALTH:
                sprite.color = new Color(1, 0.5f, 0.5f, 1);
                break;
            case type.DEFENSE:
                sprite.color = new Color(0.5f, 0.5f, 1, 1);
                break;
            default:
                sprite.color = new Color(0.5f, 1, 0.5f, 1);
                break;
        }
    }

    void OnTriggerEnter2D (Collider2D other) {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null) OnPlayerEnter(player);
    }

    void OnPlayerEnter(PlayerController player) {
        if(collected) return;
        player.ChipCollected(Type);
        Destroy(gameObject);
    }
}

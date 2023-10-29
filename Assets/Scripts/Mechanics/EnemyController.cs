using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        internal Health health;
        internal Vector3 startPos;

        protected PlayerController player;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        public ChipInstance.type type;

        public ChipInstance chip;

        protected void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            health = GetComponent<Health>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            startPos = transform.position;
        }

        protected void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.gameObject.tag == "Player") {
                if (player != null)
                {
                    var ev = Schedule<PlayerEnemyCollision>();
                    ev.player = player;
                    ev.enemy = this;
                }
            }
            else{
                Debug.Log(collision.gameObject.tag);
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision) {
            if(collision.gameObject.tag == "PlayerProjectile") {
                health.Decrement(player.projectileDamage);
                Destroy(collision.gameObject);
            }
        }

        protected void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

        public void Die() {
            StartCoroutine(Death());
            control.PlayDeathAnimation();
            ChipInstance drop = Instantiate(chip, transform.position, transform.rotation);
            drop.Type = type;
        }

        public IEnumerator Death() {
            yield return new WaitForSeconds(1);
            Destroy(gameObject);
        }
    }
}
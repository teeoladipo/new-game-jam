using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Unity.VisualScripting;
using UnityEditor.UIElements;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 3;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        public ProjectileBehavior ProjectilePrefab;
        public Transform LaunchOffset;
        public Transform AttackPoint;

        public PlayerCloseCombat closeCombat;

        SpriteRenderer sprite;

        public float invincibilityFrames = 1f;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        // player stats

        public int Defense = 0;

        // abilities related fields

        public float projectileCooldown = 0.4f;
        public float projectileDamage = 1;
        public bool StrongerGravity = false;

        public bool CloseCombatEnabled = false;

        public bool SlowFall = false;

        public bool isInvincible = false;
        public bool isSlowFalling = false;

        /*public bool isFlying = false;
        public float verticalFlySpeed = 1f;*/
        public float slowFallSpeed = 0.5f;


        // abilities private attributes

        private bool projectileAvailable = true;

        public Transform SavePoint = null;


        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            closeCombat = GetComponent<PlayerCloseCombat>();
            sprite = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
                else if(jumpState == JumpState.InFlight) {
                    if(Input.GetButton("Jump"))
                        isSlowFalling = true;
                }
                
                if (Input.GetKeyDown(KeyCode.F)) {
                    FireProjectile();
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (isSlowFalling && velocity.y < 0) {
                isSlowFalling = false;
                velocity.y -= (velocity.y + slowFallSpeed) * model.flightAcceleration;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f) {
                spriteRenderer.flipX = false;
                LaunchOffset.localPosition = new Vector3(Mathf.Abs(LaunchOffset.localPosition.x), LaunchOffset.localPosition.y, LaunchOffset.localPosition.z);
                LaunchOffset.transform.rotation = Quaternion.Euler(0, 0, -90);
                AttackPoint.localPosition = new Vector3(Mathf.Abs(LaunchOffset.localPosition.x), LaunchOffset.localPosition.y, LaunchOffset.localPosition.z);
                AttackPoint.transform.rotation = Quaternion.identity;
            }
            else if (move.x < -0.01f) {
                spriteRenderer.flipX = true;
                LaunchOffset.localPosition = new Vector3(-1 * Mathf.Abs(LaunchOffset.localPosition.x), LaunchOffset.localPosition.y, LaunchOffset.localPosition.z);
                LaunchOffset.transform.rotation = Quaternion.Euler(0, 180, -90);
                AttackPoint.localPosition = new Vector3(-1 * Mathf.Abs(LaunchOffset.localPosition.x), LaunchOffset.localPosition.y, LaunchOffset.localPosition.z);
                AttackPoint.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        private void FireProjectile() {
            if(!projectileAvailable)
                return;

            Instantiate(ProjectilePrefab, LaunchOffset.position, LaunchOffset.rotation);

            StartCoroutine(ProjectileCooldown());
        }

        private IEnumerator ProjectileCooldown() {
            projectileAvailable = false;

            yield return new WaitForSeconds(projectileCooldown);

            projectileAvailable = true;
        }

        public void KnockBack(Vector2 dir) {
            jumpState = JumpState.PrepareToJump;
            //transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            Bounce(dir);
            //velocity.y = dir.y;
        }

        public void ChipCollected(ChipInstance.type type){
            if(type == ChipInstance.type.HEALTH) {
                health.Increment();
            }
            else if (type == ChipInstance.type.DEFENSE) {
                Defense++;
            }
            switch(type) {
                case ChipInstance.type.HEALTH:
                    health.Increment();
                    break;
                case ChipInstance.type.DEFENSE:
                    Defense++;
                    break;
                case ChipInstance.type.STAT_PROJ_SPD:
                    projectileCooldown *= 0.8f;
                    break;
                case ChipInstance.type.STAT_PROJ_DMG:
                    projectileDamage += 0.25f;
                    break;
                case ChipInstance.type.STAT_ATK_SPD:
                    closeCombat.attackRate *= 0.8f;
                    break;
                case ChipInstance.type.STAT_ATK_DMG:
                    closeCombat.MeleeDamage += 0.5f;
                    break;
                case ChipInstance.type.STAT_MVMT_SPD:
                    maxSpeed += 0.5f;
                    break;
            }
        }

        public void Damage(int damage = 1) {
            if(isInvincible || !health.IsAlive)
                return;
            health.Decrement(damage);
            audioSource.PlayOneShot(ouchAudio);
            StartCoroutine(InvincibleAfterHit());
        }

        public IEnumerator InvincibleAfterHit() {
            isInvincible = true;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.7f);
            yield return new WaitForSeconds(invincibilityFrames);
            isInvincible = false;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}
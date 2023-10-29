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
    public class EnemyPursuitController : EnemyController
    {
        public float speed = 1.5F;
        public float chaseRange = 5;
        public bool canFly = false;
        
        new protected void Update()
        {
            if (player != null)
            {
                if (health.IsAlive)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) < chaseRange)
                    {
                        Chase();
                        FaceDirection();
                    }
                    else
                    {
                        ReturnPos();
                    }
                }
            }
        }
        
        private void Chase()
        {
            Vector2 targetXPos = new Vector2(player.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetXPos, speed * Time.deltaTime);
            control.velocity.x = Mathf.Sign(player.transform.position.x - transform.position.x);

            if (canFly) {
                Vector2 targetYPos = new Vector2(transform.position.x, player.transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetYPos, speed / 2 * Time.deltaTime);
            }
        }

        private void FaceDirection()
        {
            if (transform.position.x > player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        private void ReturnPos()
        {
            Vector2 targetXPos = new Vector2(startPos.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetXPos, speed * Time.deltaTime);

            if (canFly)
            {
                Vector2 targetYPos = new Vector2(transform.position.x, startPos.y);
                transform.position = Vector2.MoveTowards(transform.position, targetYPos, speed / 2 * Time.deltaTime);
            }
        }

    }
}
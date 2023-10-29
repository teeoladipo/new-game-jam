using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        IEnumerator Knockback()
        {
            float step = 25;
            for (float alpha = 10f; alpha >= 0; alpha -= 0.1f)
            {
                Vector2 targetXPos = new Vector2(enemy.transform.position.x, player.transform.position.y);
                player.transform.position = Vector2.MoveTowards(player.transform.position, targetXPos, -step * Time.deltaTime);
                step /= 1.1f;
                yield return null;
            }
        }
        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;

            if (willHurtEnemy && player.StrongerGravity)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        //Schedule<EnemyDeath>().enemy = enemy;
                        player.Bounce(2);
                    }
                    else
                    {
                        player.Bounce(7);
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                    player.Bounce(2);
                }
            }
            else if (player.GetComponent<Health>().IsAlive)
            {
                //player.GetComponent<Health>().Decrement();
                //player.Teleport(new Vector3(player.transform.position.x + Mathf.Sign(player.transform.position.x - enemy.transform.position.x) * 0.8f, player.transform.position.y, player.transform.position.z));
                //player.Bounce(7);
                //player.KnockBack(new Vector2(player.transform.position.x + Mathf.Sign(player.transform.position.x - enemy.transform.position.x) * 3f, 3f));
                player.Damage();
                StaticCoroutine.StartCoroutine(Knockback());
            }
        }
    }
}
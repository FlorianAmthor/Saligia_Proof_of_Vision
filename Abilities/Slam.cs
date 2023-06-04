using System.Collections;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [CreateAssetMenu(fileName = "Slam", menuName = "Abilities/Slam")]
    public class Slam : Ability
    {
        [SerializeField] private float _damage;
        [field: SerializeField] public float Radius { get; private set; }
        [SerializeField] private float _channelTime;

        public override IEnumerator Use(BaseUnit owner = null, params object[] args)
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            var enemy = owner.GetComponent<EnemyUnit>();
            //enemy.Animator.SetTrigger("Stand");
            //Wait for channeling and show target of boss for player
            yield return new WaitForSeconds(_channelTime);
            //enemy.Animator.SetTrigger("Stomp");
            cooldown.SetBase();
            if (Vector3.Distance(player.transform.position, owner.transform.position) <= Radius)
            {
                DamageMessage msg;

                msg.damage = _damage;
                msg.damager = enemy;
                msg.direction = player.transform.position;
                msg.damageSource = player.transform.position;

                player.ApplyDamage(msg);
            }
            var boss = owner.GetComponent<BossEnemyUnit>();
            boss.SlamAbilityOngoing = false;
            boss.SetAbilityIndicatorActive(false);
            yield return "";
        }
    }
}
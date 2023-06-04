using System.Collections;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [CreateAssetMenu(fileName = "Leap", menuName = "Abilities/Leap")]
    public class Leap : Ability
    {
        [field: SerializeField]
        public float MinRange { get; private set; }

        [field: SerializeField]
        public float MaxRange { get; private set; }

        [SerializeField] private float _initialAngle;
        [SerializeField] private float _channelTime;
        [field: SerializeField] public float Radius { get; private set; }
        [SerializeField] private float _damage;

        private Player _player;

        public override IEnumerator Use(BaseUnit owner = null, params object[] args)
        {
            var indicator = (RectTransform)args[0];
            var enemy = (EnemyUnit)owner;
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            Vector3 p = _player.transform.position;
            indicator.transform.position = p;
            //enemy.Animator.SetTrigger("Stand");
            //Wait for channeling and show target of boss for player
            yield return new WaitForSeconds(_channelTime);
            //enemy.Animator.SetTrigger("JumpAttack");
            cooldown.SetBase();

            var rigid = owner.GetComponent<Rigidbody>();

            float gravity = Physics.gravity.magnitude;
            // Selected angle in radians
            float angle = _initialAngle * Mathf.Deg2Rad;

            // Positions of this object and the target on the same plane
            Vector3 planarTarget = new Vector3(p.x, 0, p.z);
            Vector3 planarPostion = new Vector3(owner.transform.position.x, 0, owner.transform.position.z);

            // Planar distance between objects
            float distance = Vector3.Distance(planarTarget, planarPostion);
            // Distance along the y axis between objects
            float yOffset = planarPostion.y - p.y;

            float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

            // Rotate our velocity to match the direction between the two objects
            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > owner.transform.position.x ? 1 : -1);
            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

            // Fire!
            //rigid.velocity = finalVelocity;

            // Alternative way:
            rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);

            while ((owner.transform.position - p).magnitude >= 1f)
            {
                indicator.transform.position = p;
                yield return new WaitForEndOfFrame();
            }

            if (Vector3.Distance(_player.transform.position, owner.transform.position) <= Radius)
            {
                DamageMessage msg;
                msg.damage = _damage;
                msg.damager = enemy;
                msg.direction = _player.transform.position;
                msg.damageSource = _player.transform.position;

                _player.ApplyDamage(msg);
            }

            rigid.velocity = Vector3.zero;
            var boss = owner.GetComponent<BossEnemyUnit>();
            boss.LeapAbilityOngoing = false;
            boss.SetAbilityIndicatorActive(false);
        }
    }
}
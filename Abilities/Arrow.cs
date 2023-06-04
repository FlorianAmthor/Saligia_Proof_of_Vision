using Gamekit3D;
using UnityEngine;

namespace SaligiaProofOfVision
{
    public class Arrow : Projectile
    {
        public override void InitialSetup(Vector3 dir, BaseUnit owner, float impactDamage, float speed, float timeToLive = -1, params object[] data)
        {
            base.InitialSetup(dir, owner, impactDamage, speed, timeToLive);
            transform.Rotate(Vector3.up, Vector3.SignedAngle(transform.forward, direction, Vector3.up));
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            gameObject.SetActive(false);
            var player = collision.gameObject.GetComponent<Player>();
            if (collision.gameObject.CompareTag("Player"))
            {
                DamageMessage msg;
                msg.damage = impactDamage;
                msg.damager = this;
                msg.direction = (collision.collider.transform.position - transform.position).normalized;
                msg.damageSource = transform.position;

                player.ApplyDamage(msg);
            }
        }
    }
}
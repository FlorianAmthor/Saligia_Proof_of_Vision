using UnityEngine;

namespace SaligiaProofOfVision
{
    public class RangedEnemyUnit : EnemyUnit
    {
        [SerializeField] private string _projectileTag;
        [SerializeField] private float _projectileSpeed;

        public override void AttackBegin()
        {
            throw new System.NotImplementedException();
        }

        public override void AttackEnd()
        {
            throw new System.NotImplementedException();
        }

        new protected void Start()
        {
            base.Start();
        }

        //public override void Attack(IDamageable damageable)
        //{
        //    if (lastTimeAttacked + (1 / attackSpeed) > Time.time)
        //        return;
        //    animator.SetTrigger("Attack");
        //    var playerDir = player.transform.position - transform.position;
        //    playerDir.y = 0;
        //    playerDir.Normalize();

        //    //var projectile = ObjectPooler.Instance.SpawnFromPool(_projectileTag, transform.position + playerDir, Quaternion.identity).GetComponent<Projectile>();
        //    //projectile.InitialSetup(playerDir, AttackDamage, _projectileSpeed, 5);
        //    lastTimeAttacked = Time.time;
        //}
    }
}
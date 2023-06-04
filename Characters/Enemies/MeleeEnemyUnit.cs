using Gamekit3D;
using UnityEngine;

namespace SaligiaProofOfVision
{
    public class MeleeEnemyUnit : EnemyUnit
    {
        [Space(10), SerializeField] public MeleeWeapon meleeWeapon;

        protected override void Start()
        {
            base.Start();
            meleeWeapon.Init(gameObject);
            meleeWeapon.attackAudio = AttackAudio;
            meleeWeapon.damage = AttackDamage;
            Weapon = meleeWeapon;
            SceneLinkedSMB<MeleeEnemyUnit>.Initialise(controller.animator, this);
        }

        #region Animation Methods
        /// <summary>
        /// Called by animation events.
        /// </summary>
        /// <param name="frontFoot">Has a value of 1 when it's a front foot stepping and 0 when it's a back foot.</param>
        void PlayStep(int frontFoot)
        {
            if (FrontStepAudio != null && frontFoot == 1)
                FrontStepAudio.PlayRandomClip();
            else if (BackStepAudio != null && frontFoot == 0)
                BackStepAudio.PlayRandomClip();
        }
        #endregion

        public override void UpdateBonus()
        {
            base.UpdateBonus();
            meleeWeapon.damage = AttackDamage;
        }

        public override void AttackBegin()
        {
            lastTimeAttacked = Time.time;
            //Set hashInAttack True if not already set by another behaviour before this one
            meleeWeapon.BeginAttack();
        }

        public override void AttackEnd()
        {
            meleeWeapon.EndAttack();
            controller.animator.SetBool(hashInAttack, false);
            //reset hashHit bool if we just exited the attack mode
            controller.animator.SetBool(hashHit, false);
            controller.animator.SetBool(hashInvulnerableHit, false);
        }
    }
}
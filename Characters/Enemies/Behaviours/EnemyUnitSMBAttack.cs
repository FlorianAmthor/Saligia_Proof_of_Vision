using SaligiaProofOfVision;
using UnityEngine;

namespace Gamekit3D
{
    public class EnemyUnitSMBAttack : SceneLinkedSMB<EnemyUnit>
    {
        protected Vector3 m_AttackPosition;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            if (m_MonoBehaviour.Weapon.WeaponType == WeaponType.Melee)
            {
                ((MeleeWeapon)m_MonoBehaviour.Weapon).damage = m_MonoBehaviour.AttackDamage;
            }
            else
            {
                //TODO: Range weapon dmg or do nothing because of projectile?
            }

            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(false);

            m_AttackPosition = m_MonoBehaviour.Target.transform.position;
            Vector3 toTarget = m_AttackPosition - m_MonoBehaviour.transform.position;
            toTarget.y = 0;

            m_MonoBehaviour.transform.forward = toTarget.normalized;
            m_MonoBehaviour.Controller.SetForward(m_MonoBehaviour.transform.forward);

            if (m_MonoBehaviour.AttackAudio != null)
                m_MonoBehaviour.AttackAudio.PlayRandomClip();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            if (m_MonoBehaviour.AttackAudio != null)
                m_MonoBehaviour.AttackAudio.audioSource.Stop();

            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
        }
    }
}
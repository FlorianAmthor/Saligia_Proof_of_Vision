using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonWarriorUnitSMBShieldAttack : SceneLinkedSMB<SkeletonWarriorUnit>
    {
        protected Vector3 m_AttackPosition;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            m_MonoBehaviour.SetNewHitAngle(360);
            animator.SetBool(SkeletonWarriorUnit.hashShieldAttack, false);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            m_MonoBehaviour.shieldMeleeWeapon.damage = m_MonoBehaviour.ShieldAttackDamage;

            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(false);

            m_AttackPosition = m_MonoBehaviour.Target.transform.position;
            Vector3 toTarget = m_AttackPosition - m_MonoBehaviour.transform.position;
            toTarget.y = 0;

            m_MonoBehaviour.transform.forward = toTarget.normalized;
            m_MonoBehaviour.Controller.SetForward(m_MonoBehaviour.transform.forward);

            if (m_MonoBehaviour.ShieldAttackAudio != null)
                m_MonoBehaviour.ShieldAttackAudio.PlayRandomClip();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            if (m_MonoBehaviour.ShieldAttackAudio != null)
                m_MonoBehaviour.ShieldAttackAudio.audioSource.Stop();

            animator.SetBool(EnemyUnit.hashInAttack, false);
            m_MonoBehaviour.ResetHitAngle();
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
        }
    }
}
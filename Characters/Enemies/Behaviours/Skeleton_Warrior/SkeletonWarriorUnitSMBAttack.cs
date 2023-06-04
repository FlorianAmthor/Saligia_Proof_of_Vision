using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonWarriorUnitSMBAttack : SceneLinkedSMB<SkeletonWarriorUnit>
    {
        protected Vector3 m_AttackPosition;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            m_MonoBehaviour.SetNewHitAngle(360);

            m_MonoBehaviour.meleeWeapon.damage = m_MonoBehaviour.AttackDamage;

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
            animator.SetBool(EnemyUnit.hashInAttack, false);
            if (m_MonoBehaviour.AttackAudio != null)
                m_MonoBehaviour.AttackAudio.audioSource.Stop();

            m_MonoBehaviour.ResetHitAngle();
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
        }
    }
}
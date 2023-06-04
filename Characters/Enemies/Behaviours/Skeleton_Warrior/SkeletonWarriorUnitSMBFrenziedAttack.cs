using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonWarriorUnitSMBFrenziedAttack : SceneLinkedSMB<SkeletonWarriorUnit>
    {
        protected Vector3 m_AttackPosition;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            m_MonoBehaviour.SetNewHitAngle(360);
            m_MonoBehaviour.lastFrenziedAttackTime = Time.time;

            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(false);

            m_AttackPosition = m_MonoBehaviour.Target.transform.position;
            Vector3 toTarget = m_AttackPosition - m_MonoBehaviour.transform.position;
            toTarget.y = 0;

            m_MonoBehaviour.transform.forward = toTarget.normalized;
            m_MonoBehaviour.Controller.SetForward(m_MonoBehaviour.transform.forward);

            if (m_MonoBehaviour.FrenziedAttackAudio != null)
                m_MonoBehaviour.FrenziedAttackAudio.PlayRandomClip();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            if (m_MonoBehaviour.FrenziedAttackAudio != null)
                m_MonoBehaviour.FrenziedAttackAudio.audioSource.Stop();

            m_MonoBehaviour.ResetHitAngle();
            animator.SetBool(EnemyUnit.hashInAttack, false);
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
        }
    }
}
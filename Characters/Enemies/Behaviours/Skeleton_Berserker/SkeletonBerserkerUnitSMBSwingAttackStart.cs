using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonBerserkerUnitSMBSwingAttackStart : SceneLinkedSMB<SkeletonBerserkerUnit>
    {
        protected Vector3 _attackPosition;
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(false);

            _attackPosition = m_MonoBehaviour.Target.transform.position;
            Vector3 toTarget = _attackPosition - m_MonoBehaviour.transform.position;
            toTarget.y = 0;

            m_MonoBehaviour.transform.forward = toTarget.normalized;
            m_MonoBehaviour.Controller.SetForward(m_MonoBehaviour.transform.forward);

            if (m_MonoBehaviour.SwingAttackAudio != null)
                m_MonoBehaviour.SwingAttackAudio.PlayRandomClip();
        }
    }
}
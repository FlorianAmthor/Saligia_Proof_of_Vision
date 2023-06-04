using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonBerserkerUnitSMBEndSwingAttack : SceneLinkedSMB<SkeletonBerserkerUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            if (m_MonoBehaviour.SwingAttackAudio != null)
                m_MonoBehaviour.SwingAttackAudio.audioSource.Stop();
            m_MonoBehaviour.SwingAttack();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
        }
    }
}
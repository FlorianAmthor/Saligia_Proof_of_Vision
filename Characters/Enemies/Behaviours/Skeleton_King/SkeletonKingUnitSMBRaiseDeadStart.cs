using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonKingUnitSMBRaiseDeadStart : SceneLinkedSMB<SkeletonKingUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(SkeletonKingUnit.hashRaiseDead);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            base.OnSLStateEnter(animator, stateInfo, layerIndex);

            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(false);

            if (m_MonoBehaviour.RaiseDeadAudio != null)
                m_MonoBehaviour.RaiseDeadAudio.PlayRandomClip();
        }
    }
}
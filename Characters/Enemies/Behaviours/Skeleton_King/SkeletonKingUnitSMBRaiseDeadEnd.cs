using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonKingUnitSMBRaiseDeadEnd : SceneLinkedSMB<SkeletonKingUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            m_MonoBehaviour.RaiseDead();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyUnit.hashInAttack, false);
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
            if (m_MonoBehaviour.RaiseDeadAudio != null)
                m_MonoBehaviour.RaiseDeadAudio.audioSource.Stop();
        }
    }
}
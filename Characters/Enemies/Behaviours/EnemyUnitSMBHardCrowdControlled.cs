using SaligiaProofOfVision;
using UnityEngine;

namespace Gamekit3D
{
    public class EnemyUnitSMBHardCrowdControlled : SceneLinkedSMB<EnemyUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(false);
            m_MonoBehaviour.SetNewHitAngle(360f);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            m_MonoBehaviour.Controller.SetFollowNavmeshAgent(true);
            m_MonoBehaviour.ResetHitAngle();
        }
    }
}
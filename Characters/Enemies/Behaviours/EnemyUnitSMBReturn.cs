using SaligiaProofOfVision;
using UnityEngine;

namespace Gamekit3D
{
    public class EnemyUnitSMBReturn : SceneLinkedSMB<MeleeEnemyUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.WalkBackToBase();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            m_MonoBehaviour.FindTarget();

            if (m_MonoBehaviour.Target != null)
                m_MonoBehaviour.StartPursuit(); // if the player got back in our vision range, resume pursuit!
            else
                m_MonoBehaviour.WalkBackToBase();
        }
    }
}
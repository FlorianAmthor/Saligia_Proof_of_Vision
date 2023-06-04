using SaligiaProofOfVision;
using UnityEngine;

namespace Gamekit3D
{
    public class EnemyUnitSMBHit : SceneLinkedSMB<MeleeEnemyUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //animator.ResetTrigger(EnemyUnit.hashAttack);
            //m_MonoBehaviour.AttackEnd();
            animator.SetBool(EnemyUnit.hashHit, false);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Controller.ClearForce();
        }
    }
}
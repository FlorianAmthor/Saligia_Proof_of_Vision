using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonWarriorUnitSMBShieldHit : SceneLinkedSMB<SkeletonWarriorUnit>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(EnemyUnit.hashInvulnerableHit, false);

            Vector3 toTarget = m_MonoBehaviour.Target.transform.position - m_MonoBehaviour.transform.position;
            toTarget.y = 0;

            m_MonoBehaviour.transform.forward = toTarget.normalized;
            m_MonoBehaviour.Controller.SetForward(m_MonoBehaviour.transform.forward);

            if (m_MonoBehaviour.CanShieldAttack)
                animator.SetBool(SkeletonWarriorUnit.hashShieldAttack, true);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Controller.ClearForce();
        }
    }
}
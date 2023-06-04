using UnityEngine;
using UnityEngine.AI;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonKingUnitSMBPursuit : SceneLinkedSMB<SkeletonKingUnit>
    {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            m_MonoBehaviour.FindTarget();

            if (m_MonoBehaviour.Controller.navmeshAgent.pathStatus == NavMeshPathStatus.PathPartial
                || m_MonoBehaviour.Controller.navmeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                m_MonoBehaviour.StopPursuit();
                return;
            }

            if (m_MonoBehaviour.Target == null || m_MonoBehaviour.Target.IsRespawning)
            {//if the target was lost or is respawning, we stop the pursit
                m_MonoBehaviour.StopPursuit();
            }
            else
            {
                m_MonoBehaviour.RequestTargetPosition();

                Vector3 toTarget = m_MonoBehaviour.Target.transform.position - m_MonoBehaviour.transform.position;

                if (m_MonoBehaviour.CanRaiseDead)
                {
                    animator.SetTrigger(SkeletonKingUnit.hashRaiseDead);
                    animator.SetBool(EnemyUnit.hashInAttack, true);
                }
                else if (toTarget.sqrMagnitude < m_MonoBehaviour.AttackRange * m_MonoBehaviour.AttackRange)
                {
                    //TODO: if the unit cant attack it should probably switch into Idle or a similar state where it doesnt move if the player is in range
                    if (m_MonoBehaviour.CanAttackAgain)
                        m_MonoBehaviour.TriggerAttack();
                }
                else if (m_MonoBehaviour.FollowerData.assignedSlot != -1)
                {
                    Vector3 targetPoint = m_MonoBehaviour.Target.transform.position +
                        m_MonoBehaviour.FollowerData.distributor.GetDirection(m_MonoBehaviour.FollowerData
                            .assignedSlot) * m_MonoBehaviour.AttackRange * 0.9f;

                    m_MonoBehaviour.Controller.SetTarget(targetPoint);
                }
                else
                {
                    m_MonoBehaviour.StopPursuit();
                }
            }
        }
    }
}
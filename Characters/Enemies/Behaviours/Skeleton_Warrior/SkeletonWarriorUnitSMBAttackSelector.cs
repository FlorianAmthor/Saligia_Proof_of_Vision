using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonWarriorUnitSMBAttackSelector : SceneLinkedSMB<SkeletonWarriorUnit>
    {
        private int _hashAttacktrigger;
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);

            animator.ResetTrigger(SkeletonWarriorUnit.hashFrenziedAttack);
            animator.ResetTrigger(SkeletonWarriorUnit.hashDefaultAttack);

            if (m_MonoBehaviour.CanFrenzyAttack)
                _hashAttacktrigger = SkeletonWarriorUnit.hashFrenziedAttack;
            else
                _hashAttacktrigger = SkeletonWarriorUnit.hashDefaultAttack;
            animator.SetBool(EnemyUnit.hashInAttack, true);
            animator.SetTrigger(_hashAttacktrigger);
        }
    }
}
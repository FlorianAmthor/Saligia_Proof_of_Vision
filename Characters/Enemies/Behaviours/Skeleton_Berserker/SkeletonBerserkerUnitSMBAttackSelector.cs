using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonBerserkerUnitSMBAttackSelector : SceneLinkedSMB<SkeletonBerserkerUnit>
    {
        private int _hashAttacktrigger;
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);

            animator.ResetTrigger(SkeletonBerserkerUnit.hashSwingAttack);
            animator.ResetTrigger(SkeletonBerserkerUnit.hashDefaultAttack);

            if (m_MonoBehaviour.CanSwingAttack)
                _hashAttacktrigger = SkeletonBerserkerUnit.hashSwingAttack;
            else
                _hashAttacktrigger = SkeletonBerserkerUnit.hashDefaultAttack;
            animator.SetBool(EnemyUnit.hashInAttack, true);
            animator.SetTrigger(_hashAttacktrigger);
        }
    }
}
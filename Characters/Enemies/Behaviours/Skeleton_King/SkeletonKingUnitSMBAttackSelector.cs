using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonKingUnitSMBAttackSelector : SceneLinkedSMB<SkeletonKingUnit>
    {
        private int _numAttackAnimations;
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _numAttackAnimations = m_MonoBehaviour.NumAttackAnimations;
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(EnemyUnit.hashInAttack, true);
            int s = Random.Range(0, _numAttackAnimations);
            animator.SetInteger(SkeletonKingUnit.hashAttackIndex, s);
        }
    }
}
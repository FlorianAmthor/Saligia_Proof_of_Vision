using UnityEngine;

namespace SaligiaProofOfVision.Enemies.StateMachineBehaviours
{
    public class SkeletonBerserkerUnitSMBSwingAttackLoop : SceneLinkedSMB<SkeletonBerserkerUnit>
    {
        private float _swingStartTime;
        private float _swingTime;
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            _swingStartTime = Time.time;
            _swingTime = m_MonoBehaviour.SwingTime;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            if (_swingStartTime + _swingTime <= Time.time)
                animator.SetTrigger(SkeletonBerserkerUnit.hashEndSwingAttack);
        }
    }
}
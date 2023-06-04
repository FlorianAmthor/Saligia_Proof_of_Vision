using SaligiaProofOfVision;
using UnityEngine;

namespace Gamekit3D
{
    public class EnemyUnitSMBDie: SceneLinkedSMB<MeleeEnemyUnit>
    {
        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<ReplaceWithRagdoll>().Replace();
        }
    }
}
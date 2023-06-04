namespace SaligiaProofOfVision.Enemies
{

    public class SkeletonMageUnit : MeleeEnemyUnit
    {
        //#region Animator Hashes
        //public static readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");
        //#endregion

        //#region Animation Parameters
        //[SerializeField] private Animation[] _defaultAttackAnimations;
        //#endregion

        protected override void Start()
        {
            base.Start();
            SceneLinkedSMB<SkeletonMageUnit>.Initialise(controller.animator, this);
        }

        //public override void AttackBegin()
        //{
        //    if (_defaultAttackAnimations.Length > 1)
        //        controller.animator.SetInteger(hashAttackIndex, Random.Range(0, _defaultAttackAnimations.Length - 1));
        //    else
        //        controller.animator.SetInteger(hashAttackIndex, 0);
        //    base.AttackBegin();
        //}
    }
}
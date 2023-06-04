namespace SaligiaProofOfVision
{
    public class GhoulBossUnit : MeleeEnemyUnit
    {
        new protected void Start()
        {
            base.Start();
            SceneLinkedSMB<GhoulBossUnit>.Initialise(controller.animator, this);
        }
    }
}
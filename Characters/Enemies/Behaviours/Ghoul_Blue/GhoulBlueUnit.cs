namespace SaligiaProofOfVision
{
    public class GhoulBlueUnit : MeleeEnemyUnit
    {
        new protected void Start()
        {
            base.Start();
            SceneLinkedSMB<GhoulBlueUnit>.Initialise(controller.animator, this);
        }
    }
}